# Testing Strategy

This document describes the testing approach for the Law and Duty mod, including automated unit tests and manual integration tests required due to dependencies on GTA V game engine.

---

## Unit Tests

Unit tests are implemented using NUnit 3.5.0 and can be run from any test runner (Visual Studio, ReSharper, Rider, etc.).

### Current Test Coverage

#### GameState (`LawAndDuty.Tests/Core/GameStateTests.cs`)

✅ **Fully covered** - All functionality tested with nominal and edge cases:

- `Constructor_DefaultValues_InitializesCorrectly`: Verifies default initialization
- `HasReceivedDaveInitialMessage_SetToTrue_StoresCorrectly`: Tests property setter
- `HasReceivedDaveInitialMessage_SetToFalse_StoresCorrectly`: Tests property reset
- `GameState_MultipleInstances_AreIndependent`: Ensures instance independence
- `GameState_PropertyToggling_WorksCorrectly`: Tests multiple state changes

### Running Unit Tests

```bash
# Using .NET CLI (if available)
dotnet test LawAndDuty.Tests/LawAndDuty.Tests.csproj

# Or build and run through your IDE's test runner
```

---

## Integration Tests

Integration tests cannot be automated due to dependencies on ScriptHookVDotNet3 and GTA V game engine (native functions, `Script` base class, `Game.Player`, etc.). These must be performed manually in-game.

### Why Integration Tests Are Manual

The following classes depend on GTA V runtime and cannot be unit tested without extensive refactoring:

- **LawAndDutyScript**: Inherits from `Script` (SHVDN3)
- **InitialMessageScript**: Uses `Game.Player.Character`, `Notification.Show`, native game functions
- **Locations**: Depends on `Vector3` from GTA.Math

To make these testable, we would need:
1. Interface wrappers for all GTA services (`IGameService`, `INotificationService`, `IScriptManager`)
2. Dependency injection throughout the codebase
3. Mock implementations for testing

**Decision**: Given the current project scope, manual integration testing is more practical than architecture refactoring.

---

## Manual Integration Test Procedures

### Test 1: Initial Message System - First Launch

**Purpose**: Verify that Dave's message is sent correctly when Michael reaches his poolside for the first time.

**Prerequisites**:
- Fresh game save or deleted `scripts/Waldhari/LawAndDuty.xml`
- Mod installed correctly in `scripts/` folder
- Waldhari.Core.dll present in `scripts/Waldhari/`

**Steps**:
1. Load GTA V with the mod installed
2. Play as Michael
3. Navigate to Michael's house
4. Walk to the poolside area (coordinates defined in `Constants/Locations.cs`)
5. Wait near the pool for ~5 seconds

**Expected Results**:
- ✅ Dave Norton's SMS appears on screen (top-left notification)
- ✅ Message text: *"Michael, c'est Dave. J'ai parlé au chef. Il accepte de te prendre, mais tu commences tout en bas - patrouilles, contraventions, le package complet. Viens au commissariat quand tu es prêt. À toi de faire tes preuves."*
- ✅ Character icon shows "Dave Norton"
- ✅ Log file `scripts/Waldhari/LawAndDuty.log` contains:
  - "InitialMessageScript instantiated"
  - "Dave's initial message sent to Michael"
  - "Game state saved: HasReceivedDaveInitialMessage = true"
- ✅ Save file `scripts/Waldhari/LawAndDuty.xml` is created with `<HasReceivedDaveInitialMessage>true</HasReceivedDaveInitialMessage>`

**Failure Indicators**:
- ❌ No message appears after waiting near pool
- ❌ Message appears multiple times
- ❌ Game crashes or script errors in log
- ❌ Save file not created or has incorrect values

---

### Test 2: Message Persistence - Subsequent Launches

**Purpose**: Verify that Dave's message is NOT sent again after the first time.

**Prerequisites**:
- Completed Test 1 successfully
- Save file exists: `scripts/Waldhari/LawAndDuty.xml` with `HasReceivedDaveInitialMessage = true`

**Steps**:
1. Quit and restart GTA V
2. Load the same save
3. Play as Michael
4. Return to the poolside area
5. Wait near the pool for ~30 seconds

**Expected Results**:
- ✅ NO message from Dave appears
- ✅ Log file shows:
  - "LawAndDuty script initializing..."
  - "GameState loaded successfully"
  - "HasReceivedDaveInitialMessage: True"
  - NO "InitialMessageScript instantiated" (script should not be created)
- ✅ Game continues normally without errors

**Failure Indicators**:
- ❌ Message appears again (persistence failure)
- ❌ Script crashes or errors in log
- ❌ InitialMessageScript is instantiated when it shouldn't be

---

### Test 3: Distance Threshold Detection

**Purpose**: Verify that the message only triggers within the defined proximity threshold.

**Prerequisites**:
- Fresh game save or deleted save file
- Coordinates properly defined in `Constants/Locations.cs`

**Steps**:
1. Load game as Michael
2. Approach the poolside slowly, stopping at different distances:
   - 20 meters away
   - 10 meters away
   - 6 meters away (just outside threshold)
   - 5 meters away (at threshold)
   - Inside the pool area

**Expected Results**:
- ✅ No message when >5 meters away
- ✅ Message triggers when ≤5 meters from defined poolside coordinates
- ✅ Message only appears once regardless of movement within/outside threshold

**Failure Indicators**:
- ❌ Message triggers too far from pool
- ❌ Message doesn't trigger when very close to pool
- ❌ Message triggers multiple times when crossing threshold repeatedly

---

### Test 4: Script Lifecycle & Cleanup

**Purpose**: Verify that InitialMessageScript properly terminates after completing its job.

**Prerequisites**:
- Fresh game save
- Access to log file for real-time monitoring

**Steps**:
1. Load game as Michael
2. Monitor `scripts/Waldhari/LawAndDuty.log` in real-time (e.g., `tail -f` on Linux/Mac, or text editor with auto-refresh)
3. Approach poolside and trigger the message
4. Continue playing for 5 minutes

**Expected Results**:
- ✅ Log shows "InitialMessageScript instantiated"
- ✅ Log shows "Dave's initial message sent to Michael"
- ✅ Log shows "Game state saved"
- ✅ No repeated log entries from InitialMessageScript after message is sent
- ✅ No memory leaks or performance degradation
- ✅ Script properly calls `Abort()` on itself

**Failure Indicators**:
- ❌ InitialMessageScript continues logging after message sent
- ❌ Performance issues or frame drops
- ❌ Memory usage increases over time
- ❌ Script errors indicating improper cleanup

---

### Test 5: Localization System

**Purpose**: Verify that localization service loads the correct French message.

**Prerequisites**:
- Fresh game save
- `Properties/LawAndDuty/fr-FR/General.csv` exists with correct content

**Steps**:
1. Load game and trigger Dave's message
2. Verify message content matches exactly

**Expected Results**:
- ✅ Message displays in French
- ✅ Message text exactly matches `dave_initial_message` key value in CSV
- ✅ No placeholder keys (e.g., "dave_initial_message") displayed
- ✅ Log shows: "Message retrieved for key 'dave_initial_message'"

**Failure Indicators**:
- ❌ Message shows key instead of translated text
- ❌ Message is in wrong language
- ❌ Message has formatting issues or encoding problems
- ❌ Log shows warnings about missing translation

---

### Test 6: Save/Load State Integrity

**Purpose**: Verify that game state persists correctly through save/load cycles.

**Prerequisites**:
- Completed Test 1

**Steps**:
1. Trigger Dave's message (Test 1)
2. Play for a few minutes
3. Save game manually
4. Quit GTA V completely
5. Restart GTA V
6. Load the saved game
7. Return to poolside

**Expected Results**:
- ✅ Message does not appear again
- ✅ XML save file maintains correct state
- ✅ No script initialization errors in log

**Failure Indicators**:
- ❌ State is lost after save/load
- ❌ XML file is corrupted or missing values
- ❌ Message triggers again after reload

---

## Test Environment Setup

### Required Files
```
<GTA V Directory>/
├── scripts/
│   ├── LawAndDuty.dll
│   ├── ScriptHookVDotNet3.dll
│   └── Waldhari/
│       ├── Core.dll
│       └── LawAndDuty/
│           └── fr-FR/
│               └── General.csv
```

### Verification Checklist Before Testing
- [ ] All DLL files present
- [ ] Localization CSV file exists
- [ ] ScriptHookV + ScriptHookVDotNet3 installed
- [ ] .NET Framework 4.8 installed
- [ ] Fresh game save or backup of save files

---

## Future Testing Improvements

When the mod grows larger, consider:

1. **Architecture Refactoring for Testability**:
   - Introduce service interfaces (`IGameService`, `INotificationService`)
   - Implement dependency injection
   - Create mock implementations for unit testing

2. **Automated Integration Testing**:
   - Use FiveM or similar framework for automated in-game testing
   - Create test harnesses that simulate GTA V environment

3. **Continuous Integration**:
   - Set up CI pipeline (GitHub Actions)
   - Automated unit test execution on push
   - Build verification

4. **Test Coverage Tools**:
   - Integrate code coverage analysis (dotCover, Coverlet)
   - Track coverage metrics over time

---

## Reporting Issues

When reporting test failures, please include:

1. **Test name** that failed
2. **Log file content** (`scripts/Waldhari/LawAndDuty.log`)
3. **Save file content** (`scripts/Waldhari/LawAndDuty.xml`)
4. **Steps to reproduce**
5. **GTA V version** and mod versions
6. **Expected vs actual behavior**

File issues at: [https://github.com/RomainDel59/LawAndDuty/issues](https://github.com/RomainDel59/LawAndDuty/issues)

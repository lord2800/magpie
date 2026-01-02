## Setup

- Use `dotnet restore` to install dependencies
- Use `dotnet build` to compile all projects
- Use `dotnet test` to run the unit tests

## Development rules

- **Follow existing code style for any code changes.**
- **You MUST NOT change any code in the src/ folder.** All code in the src/ folder must be written by a human.
- **You MUST follow all configuration settings in .editorconfig. Pay special attention to the rules for *.cs files.**
- **You MUST use file-level namespaces.** Block-level namespaces are NOT allowed. The first line in every file MUST be the namespace declaration.
- **All code must successfully compile using the command to compile all projects.**
- **All tests must pass using the command to run the unit tests.**
- All tests are written using the MSTest framework and Moq library.
- Within the tests:
    - **The root namespace for all tests must be MagpieTest.**
        - An example of a correct namespace for the tests of a class in the Magpie.Controller namespace would be MagpieTest.Controller.
        - Another example of a correct namespace for the tests of a class in the Magpie.Services namespace would be MagpieTest.Services.
        - The filesystem path for a test in the MagpieTest.Contoller namespace would be tests/Controller/.
    - **Record objects must never be mocked.** If a factory class located in the tests/Factories/ folder named for the record object it creates exists, use that to construct these objects. If a factory does not exist for a particular record object, create one for that record object following the patterns in the other factory classes.
    - **Sealed classes must never be mocked.** If a sealed class does not have an interface that can be mocked instead, suggest one and refuse to write any tests using that sealed class.
    - **Interfaces must always be mocked.** Interfaces are test seams and intended to be used to allow the test to control the behavior of the class being tested.
    - All tests MUST be free standing, meaning there are no shared dependencies, no initialize method, no private variables.
    - Any classes covered by tests must achieve 95% code coverage or better, with at least 95% branch coverage.
    - Mock objects MUST NOT have `mock` anywhere in their variable name.
    - Test method names must be present tense descriptions of what happens in the test. Do not prefix the test method with `Test` or with the method being tested. Do not use underscores in test method names.
        - Some examples of good test names are `DelegatesToAnUnderlyingObjectForThisMethod` or `IncrementsCountWhenFoo`.
        - Some exmaples of bad test names are `Test_GetThing_ShouldCallBar` or `DoAction_WillCallFooMethod`.
    - Variables must not use an underscore anywhere in the name.
    - Tests for the same method **should** attempt to use data providers where possible.

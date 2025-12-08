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
    - **Record objects must never be mocked.** If a factory class located in the tests/Factories/ folder named for the record object it creates exists, use that to construct these objects.
    - **Sealed classes must never be mocked.**
    - **Interfaces must always be mocked.**
    - All tests should be free standing, meaning there are no shared dependencies, no initialize method, no private variables.
    - Any classes covered by tests must achieve 95% code coverage or better, with at least 95% branch coverage.
    - Mock objects MUST NOT have their variable name prefixed with mock.
    - Test method names must be present tense descriptions of what happens in the test. Do not prefix the test method with the method being tested.

# SkillBridge API Testing

This folder contains unit tests for the SkillBridge API project.

## Testing Architecture

The tests are organized as follows:

### Common

- `BaseTestFixture.cs` - Base class for test fixtures
- `DatabaseTestBase.cs` - Base class for database-related tests using in-memory database
- `MockLoggerFactory.cs` - Factory for creating mock loggers
- `RepositoryMockHelper.cs` - Helper for mocking repository operations
- `TestConstants.cs` - Constants used throughout tests
- `TestDataGenerator.cs` - Generator for test data

### Fixtures

- `MapperFixture.cs` - Fixture for configuring mapping

### Services

- `ServiceTestBase.cs` - Base class for service tests
- `SkillServiceTests.cs` - Tests for the SkillService class using in-memory database
- `SkillServiceMockTests.cs` - Tests for the SkillService class using mocks

### Data

- `AppDbContextTests.cs` - Tests for the AppDbContext class

### Infrastructure

- `MappingTests.cs` - Tests for the mapping configurations

## Running Tests

To run the tests, use:

```bash
dotnet test
```

Or run specific tests:

```bash
dotnet test --filter "FullyQualifiedName~SkillBridge.Tests.Services.SkillServiceTests"
```

## Adding New Tests

When adding new tests:

1. Decide if you need to test with an in-memory database or with mocks
2. For database tests, inherit from `DatabaseTestBase`
3. For service tests, inherit from `ServiceTestBase`
4. Use `TestDataGenerator` to create test data
5. Use `MockLoggerFactory` to create and verify log messages
6. For mocking repositories, use `RepositoryMockHelper`

## Best Practices

1. Each test should be independent and not rely on the state from other tests
2. Use descriptive test names in the format: `MethodName_Scenario_ExpectedResult`
3. Follow the AAA (Arrange-Act-Assert) pattern
4. Mock external dependencies
5. Verify that log messages are created correctly

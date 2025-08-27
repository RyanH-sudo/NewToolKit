# Contributing to NetToolkit 🤝

Thank you for your interest in contributing to **NetToolkit**! We welcome contributions from network engineers, developers, and enthusiasts who want to help build the ultimate network engineering platform.

## 📋 Code of Conduct

This project and everyone participating in it is governed by our [Code of Conduct](CODE_OF_CONDUCT.md). By participating, you are expected to uphold this code.

## 🚀 Getting Started

### Prerequisites
- **Windows 10/11** or **Windows Server 2019+**
- **.NET 8 SDK** or later
- **Visual Studio 2022** or **VS Code** with C# extension
- **Git** for version control
- **SQL Server** or **SQL Server LocalDB**

### Development Environment Setup

1. **Fork and Clone**
   ```bash
   git clone https://github.com/YourUsername/NetToolkit.git
   cd NetToolkit
   ```

2. **Install Dependencies**
   ```bash
   dotnet restore NetToolkit.sln
   dotnet tool install --global dotnet-ef
   ```

3. **Database Setup**
   ```bash
   dotnet ef database update --project src/Core/NetToolkit.Core
   ```

4. **Build and Test**
   ```bash
   dotnet build NetToolkit.sln --configuration Debug
   dotnet test --configuration Debug
   ```

## 🏗️ Project Structure

Understanding the modular architecture is key to effective contribution:

```
NetToolkit/
├── 🎯 src/Core/              # Core engine and shared interfaces
├── 🔧 src/Modules/           # Individual feature modules
│   ├── PowerShell/          # Terminal and SSH functionality
│   ├── ScannerAndTopography/ # Network scanning and 3D visualization
│   ├── Security/            # Security analysis and vulnerability scanning
│   ├── PuTTY/              # Serial/USB/Bluetooth communication
│   ├── Education/          # Learning platform with 10 modules
│   ├── AiOrb/              # AI assistant with OpenAI integration
│   ├── MicrosoftAdmin/     # Graph API and Office 365 integration
│   ├── UiPolish/           # UI enhancements and Three.js graphics
│   └── SecurityFinal/      # Final security hardening module
├── 🖥️ src/UI/               # WPF application frontend
├── 🧪 tests/               # Unit and integration tests
└── 📚 docs/                # Documentation and guides
```

## 🎯 How to Contribute

### 1. **Bug Reports** 🐛
- Use the [Issue Template](https://github.com/RyanH-sudo/NetToolkit/issues/new?template=bug_report.md)
- Include system information, steps to reproduce, and expected vs actual behavior
- Add relevant logs from `%TEMP%/NetToolkit/logs/`

### 2. **Feature Requests** 💡
- Use the [Feature Request Template](https://github.com/RyanH-sudo/NetToolkit/issues/new?template=feature_request.md)
- Describe the use case and business value
- Consider backward compatibility and performance impact

### 3. **Code Contributions** 💻

#### Development Workflow
1. **Create a Branch**
   ```bash
   git checkout -b feature/your-feature-name
   # or
   git checkout -b bugfix/issue-number-description
   ```

2. **Make Changes**
   - Follow our [Coding Standards](#coding-standards)
   - Write tests for new functionality
   - Update documentation as needed

3. **Test Your Changes**
   ```bash
   dotnet test --configuration Debug --collect:"XPlat Code Coverage"
   dotnet build NetToolkit.sln --configuration Release
   ```

4. **Commit and Push**
   ```bash
   git add .
   git commit -m "feat: add network device auto-discovery"
   git push origin feature/your-feature-name
   ```

5. **Create Pull Request**
   - Use our [PR Template](https://github.com/RyanH-sudo/NetToolkit/pull/new)
   - Link related issues
   - Include screenshots/videos for UI changes

## 📝 Coding Standards

### C# Guidelines
- Follow **Microsoft C# Coding Conventions**
- Use **async/await** for I/O operations
- Implement **proper error handling** with try-catch blocks
- Use **dependency injection** for service registration
- Write **XML documentation** for public APIs

### Architecture Principles
- **Modular Design**: Each module should be self-contained
- **SOLID Principles**: Follow single responsibility, open/closed, etc.
- **Event-Driven Architecture**: Use the event bus for cross-module communication
- **Performance First**: Sub-100ms response times, <200MB RAM usage
- **Security by Design**: Validate all inputs, secure API calls

### Testing Requirements
- **Unit Tests**: Minimum 80% code coverage
- **Integration Tests**: Test module interactions
- **Performance Tests**: Verify resource usage targets
- **UI Tests**: Automated testing for critical user flows

### Documentation Standards
- **README Updates**: Update feature lists and installation instructions
- **Code Comments**: Explain complex business logic
- **API Documentation**: Document all public interfaces
- **Change Log**: Update CHANGELOG.md for user-facing changes

## 🧪 Testing Guidelines

### Test Categories
1. **Unit Tests** (`tests/NetToolkit.*.Tests/`)
   - Test individual components in isolation
   - Mock external dependencies
   - Fast execution (<1 second per test)

2. **Integration Tests** (`tests/NetToolkit.Integration.Tests/`)
   - Test module interactions
   - Use test database and mock services
   - Medium execution time (<10 seconds per test)

3. **Performance Tests** (`tests/NetToolkit.Performance.Tests/`)
   - Validate resource usage requirements
   - Memory leak detection
   - Response time verification

### Running Tests
```bash
# All tests
dotnet test

# Specific category
dotnet test --filter "Category=Unit"
dotnet test --filter "Category=Integration"

# With coverage
dotnet test --collect:"XPlat Code Coverage"
```

## 🎨 UI/UX Contributions

### Three.js Components
- Follow **WebGL best practices** for performance
- Implement **LOD (Level of Detail)** for complex scenes
- Use **instancing** for repeated geometries
- Maintain **60fps** animation performance

### WPF Guidelines
- Use **MVVM pattern** with proper data binding
- Implement **responsive design** for different screen sizes
- Follow **Material Design** principles for consistency
- Ensure **accessibility compliance** (screen readers, keyboard navigation)

## 🔒 Security Considerations

### Security Reviews
All contributions involving:
- **Authentication/Authorization**
- **Network Communication**
- **File System Access**
- **External API Integration**
- **User Input Validation**

Must include security review and threat modeling.

### Secure Coding Practices
- **Input Validation**: Sanitize all user inputs
- **SQL Injection Prevention**: Use parameterized queries
- **XSS Prevention**: Encode output in web components
- **Secrets Management**: Never commit API keys or passwords
- **Principle of Least Privilege**: Minimal required permissions

## 📊 Performance Standards

### Resource Requirements
- **Memory Usage**: < 200MB during normal operation
- **CPU Usage**: < 5% during network scanning
- **Response Time**: < 100ms for UI interactions
- **Startup Time**: < 5 seconds on modern hardware

### Optimization Techniques
- **Lazy Loading**: Load modules on demand
- **Connection Pooling**: Reuse database connections
- **Caching**: Cache frequently accessed data
- **Async Operations**: Non-blocking I/O operations

## 🌍 Internationalization

### Multi-language Support
- Use **resource files** (.resx) for all user-facing strings
- Support **right-to-left (RTL)** languages
- Test with **pseudo-localization** during development
- Consider **cultural differences** in date/time formatting

## 📈 Monitoring and Telemetry

### Logging Standards
- Use **structured logging** with NLog
- Include **correlation IDs** for tracing requests
- Log **performance metrics** for monitoring
- Implement **different log levels** appropriately

### Error Handling
- **User-friendly error messages** with actionable guidance
- **Detailed logging** for debugging purposes
- **Graceful degradation** when services are unavailable
- **Retry mechanisms** with exponential backoff

## 🤖 Automation and CI/CD

### Continuous Integration
- **Automated testing** on every commit
- **Code quality checks** with static analysis
- **Security scanning** for vulnerabilities
- **Performance regression** detection

### Documentation
- **API documentation** generation
- **Architecture decision records** (ADRs)
- **Deployment guides** for different environments
- **Troubleshooting guides** for common issues

## 🏆 Recognition

We appreciate all contributions and recognize contributors through:

- **Contributors section** in README
- **Release notes** acknowledgments
- **Community highlights** on social media
- **Maintainer invitation** for significant contributors

## 💬 Getting Help

### Community Support
- **GitHub Discussions** for general questions
- **Discord Server** for real-time chat (coming soon)
- **Stack Overflow** with `nettoolkit` tag
- **Documentation Wiki** for detailed guides

### Maintainer Contact
- **@RyanH-sudo** - Project maintainer and architect
- Response time: Typically within 24-48 hours
- Available for **architecture discussions** and **feature planning**

---

## 🙏 Thank You

Your contributions make **NetToolkit** better for network engineers worldwide. Whether you're fixing a typo, adding a feature, or improving performance, every contribution matters!

---

*Happy coding! 🚀*
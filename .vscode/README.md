# Unity + Cursor Integration Guide

This directory contains configuration files for using Cursor (or VS Code) as your Unity development environment.

## Setup Instructions

### 1. Install Required Extensions

Cursor will prompt you to install recommended extensions when you open this project. Alternatively, you can install them manually:

**Essential Extensions:**
- **C# Dev Kit** (`ms-dotnettools.csdevkit`) - Modern C# development tools
- **C#** (`ms-dotnettools.csharp`) - C# language support
- **Unity Tools** (`visualstudiotoolsforunity.vstuc`) - Unity-specific IntelliSense and debugging

**Optional but Recommended:**
- **Unity Code Snippets** (`kleber-swf.unity-code-snippets`) - Useful Unity code snippets
- **Unity Tools** (`tobiah.unity-tools`) - Additional Unity utilities

### 2. Configure Unity to Use Cursor

1. Open your Unity project
2. Go to **Edit > Preferences > External Tools** (Windows) or **Unity > Preferences > External Tools** (Mac)
3. Under **External Script Editor**, click the browse button
4. Navigate to and select Cursor executable:
   - **Windows**: Usually `C:\Users\<YourUsername>\AppData\Local\Programs\cursor\Cursor.exe`
   - **Mac**: Usually `/Applications/Cursor.app`
   - **Linux**: Usually `/usr/bin/cursor` or wherever you installed it
5. Click **Apply**

### 3. Open C# Project from Unity

1. In Unity, go to **Assets > Open C# Project**
2. This will:
   - Generate/update the `.csproj` and `.sln` files
   - Open Cursor with the project loaded
   - Enable IntelliSense and code completion

### 4. Verify IntelliSense is Working

1. Open any `.cs` file in the `Assets/Scripts` folder
2. You should see:
   - Syntax highlighting
   - Code completion (IntelliSense)
   - Error squiggles
   - Go to Definition support

If IntelliSense isn't working:
- Wait a few moments for OmniSharp to initialize (check the status bar)
- Check the Output panel: View > Output > Select "OmniSharp Log"
- Try restarting Cursor
- Ensure Unity has generated the `.csproj` files (Assets > Open C# Project)

## Features Enabled

### Code Formatting
- Automatic formatting on save
- Consistent 4-space indentation for C# files
- Trailing whitespace removal

### IntelliSense
- Full Unity API IntelliSense
- Code completion
- Error detection
- Go to Definition
- Find References

### Debugging
- Attach to Unity Editor
- Attach to Unity Player
- Breakpoints and step-through debugging

### File Management
- Unity build artifacts excluded from file explorer
- Improved search performance (Library/Temp excluded)
- File watcher optimized for Unity projects

## Troubleshooting

### IntelliSense Not Working
1. Check that `.csproj` files exist in the project root
2. Open Output panel and check OmniSharp logs
3. Try: `Ctrl+Shift+P` > "OmniSharp: Restart OmniSharp"
4. Regenerate project files: In Unity, go to **Assets > Open C# Project**

### Extension Not Found
- Cursor uses VS Code extensions, so search for extensions in the Extensions marketplace
- Some extensions may need to be installed manually if not auto-detected

### Build Errors
- Unity must be running or have generated project files
- Check Unity Console for compilation errors
- Ensure all Unity packages are properly imported

## Additional Resources

- [Unity Documentation](https://docs.unity3d.com/)
- [C# Dev Kit Documentation](https://code.visualstudio.com/docs/languages/csharp)
- [Unity Tools Extension](https://marketplace.visualstudio.com/items?itemName=visualstudiotoolsforunity.vstuc)

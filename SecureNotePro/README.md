# SecureNote Pro - Educational Reverse Engineering Practice Application

![Educational Purpose](https://img.shields.io/badge/Purpose-Educational-blue)
![Difficulty](https://img.shields.io/badge/Difficulty-Intermediate-orange)
![.NET](https://img.shields.io/badge/.NET-8.0-purple)

## ⚠️ Educational Purpose Only

This application is **designed for educational purposes** to teach and practice reverse engineering, software protection analysis, and ethical security research. It implements various software protection techniques commonly found in commercial applications.

## 📋 Overview

SecureNote Pro is a fully functional note-taking application with premium features locked behind a license key system. It demonstrates multiple protection techniques used in real-world software:

### Application Features

**Free Features (Very Limited!):**
- ✓ Create and edit notes (max **3 notes only!**)
- ✓ Save notes (max **10 saves total!**)
- ✓ Basic note organization

**Premium Features (Locked):**
- 🔒 Tags for notes
- 🔒 Favorite marking
- 🔒 Category organization
- 🔒 Advanced search
- 🔒 Export to multiple formats (PDF, HTML, Markdown, JSON)
- ✅ Unlimited notes (remove 3 note limit)
- ✅ Unlimited saves (remove 10 save limit)

### Trial System - **VERY SHORT!**
- 🚨 **Only 2 MINUTES trial period!** - You must activate quickly!
- Real-time countdown timer with second-by-second updates
- Warning popups at 1 minute and 30 seconds remaining
- Trial expiration popup when time runs out
- Full access to premium features during trial (unlimited notes & saves)
- Trial information stored in multiple locations for redundancy
- Automatic trial tracking and validation

### Free Version Limits - **VERY RESTRICTIVE!**
- **Maximum 3 notes** - Free users can only create 3 notes maximum
- **Maximum 10 saves** - Free users can only save notes 10 times total
- Usage tracking stored across multiple locations with obfuscation
- Counters persist across application restarts
- Forces users to activate to get any real work done!

## 🛡️ Protection Techniques Implemented

This application implements the following protection mechanisms for educational analysis:

### 1. **License Key Validation**
- Custom checksum algorithm using XOR operations and bit rotation
- Multi-part validation (format, checksum, advanced algorithm, signature)
- Base-36 encoding for checksum representation
- Magic number verification (divisibility checks)
- HMAC-SHA256 signature validation
- Decoy validation functions to confuse analysis

**Location:** `Protection/LicenseValidator.cs`

**Learning Objectives:**
- Understand custom license key algorithms
- Practice tracing validation logic through multiple functions
- Learn to identify decoy/dummy functions

### 2. **Anti-Debugging Techniques**
- `IsDebuggerPresent()` Windows API check
- `CheckRemoteDebuggerPresent()` detection
- Managed debugger detection (`Debugger.IsAttached`)
- Timing-based debugger detection
- Background monitoring thread
- Hardware breakpoint detection

**Location:** `Protection/AntiDebug.cs`

**Learning Objectives:**
- Bypass various anti-debugging checks
- Understand timing-based detection methods
- Practice patching or hooking detection functions

### 3. **Code Integrity Verification**
- Assembly hash verification
- Method IL (Intermediate Language) size checking
- Detection of common patching patterns (return true/false patches)
- Debug attribute verification
- Runtime integrity checks

**Location:** `Protection/IntegrityCheck.cs`

**Learning Objectives:**
- Understand how integrity checks work
- Learn to modify code without triggering checks
- Practice IL manipulation techniques

### 4. **String Obfuscation**
- XOR encryption with rotating keys
- Base64 encoding for storage
- Runtime decryption
- Decoy decryption functions with wrong algorithms
- Indirect string access patterns

**Location:** `Protection/ObfuscatedStrings.cs`

**Learning Objectives:**
- Decrypt obfuscated strings
- Identify encryption algorithms
- Distinguish real functions from decoys

### 5. **Trial Period Management**
- Multiple storage locations (Registry + hidden files)
- Timestamp obfuscation using XOR and bit rotation
- AES encryption for stored data
- Cross-validation between multiple sources
- Anti-tamper checks (future date detection, etc.)

**Location:** `Protection/TrialManager.cs`

**Trial Data Locations:**
- Registry: `HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced\SecureNotePro`
- File 1: `%AppData%\.snp_config`
- File 2: `%LocalAppData%\Microsoft\Edge\.snp_cache`

**Learning Objectives:**
- Locate and modify trial data
- Understand timestamp obfuscation
- Bypass multi-source validation

### 6. **Feature Gating System**
- Multi-layer validation before granting feature access
- Combined license, trial, and integrity checks
- Runtime re-validation to prevent memory patching
- Indirect validation paths
- Decoy validation functions

**Location:** `Protection/FeatureGate.cs`

**Learning Objectives:**
- Trace feature unlock logic
- Bypass runtime validation checks
- Patch feature gates effectively

### 7. **Usage Tracking & Limits**
- Track note creation count (**3 max** for free users)
- Track save operation count (**10 max** for free users)
- Multiple storage locations (Registry + hidden files)
- Counter obfuscation using XOR and bit rotation
- Checksum verification to detect tampering
- Uses highest value from all sources (prevents simple reset)

**Location:** `Protection/UsageTracker.cs`

**Storage Locations:**
- Registry: `HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced\SecureNotePro`
- File 1: `%AppData%\.snp_usage`
- File 2: `%LocalAppData%\Microsoft\Edge\.snp_usage`

**Learning Objectives:**
- Locate usage tracking storage
- Understand counter obfuscation
- Reset or bypass usage limits
- Modify counters without detection

## 🔑 Valid License Keys

For educational testing, here are **valid license keys**:

```
Test Key 1:  SK4N-0G7A-4NDI-4L75
Test Key 2:  2MBL-7S5X-CNQI-3L8N
Test Key 3:  XWMQ-A39T-0VKA-0T9I
Educational: 8IM2-3LA6-WNM2-1L1D
Demo Key:    5DYL-8S3Z-XPF1-2T4Y
```

### How to Generate More Keys

Use the `GenerateLicenseKey()` method in `LicenseValidator.cs`:

```csharp
string key = LicenseValidator.GenerateLicenseKey("your-seed-string");
```

The seed can be any string - the same seed always generates the same key.

## 🎯 Reverse Engineering Challenges

### Beginner Challenges
1. **Find the Trial Location** - Locate where trial data is stored
2. **Decrypt a String** - Decrypt one of the obfuscated strings manually
3. **Identify the Decoy** - Find the decoy validation function in LicenseValidator
4. **Extract License Format** - Document the license key format requirements
5. **Find Usage Counters** - Locate where note count and save count are stored

### Intermediate Challenges
6. **Reset the Trial** - Modify trial data to reset the 30-day period
7. **Reset Usage Limits** - Reset the save counter to 0
8. **Bypass Anti-Debug** - Run the app under a debugger without triggering warnings
9. **Generate Valid Key** - Write a keygen to create valid license keys
10. **Patch Feature Gate** - Unlock a single premium feature permanently
11. **Remove Note Limit** - Modify the app to allow unlimited notes

### Advanced Challenges
12. **Universal Patch** - Create a patch that unlocks all features for any user
13. **Bypass Integrity** - Modify validation logic without triggering integrity checks
14. **Memory Patching** - Runtime patching without modifying the executable file
15. **Understand Counter Obfuscation** - Reverse engineer the counter obfuscation algorithm
16. **Create a Loader** - Build a custom loader that bypasses all protections

## 🛠️ Recommended Tools

### Debuggers
- **x64dbg** - Native debugging for Windows applications
- **dnSpy** - .NET debugger and decompiler
- **Visual Studio** - Step-through debugging

### Decompilers
- **ILSpy** - .NET decompiler (open source)
- **dnSpy** - Integrated decompiler and editor
- **dotPeek** - JetBrains .NET decompiler

### Analysis Tools
- **PE Explorer** - PE file analysis
- **Process Monitor** - Monitor registry and file access
- **Process Explorer** - Advanced process information
- **CFF Explorer** - PE editor

### Hex Editors
- **HxD** - Hex editor for binary patching
- **010 Editor** - Advanced hex editor with templates

## 📚 Building the Application

### Prerequisites
- .NET 8.0 SDK or later
- Windows operating system
- Visual Studio 2022 or JetBrains Rider (recommended)

### Build Instructions

**Option 1: Using Visual Studio**
1. Open `SecureNotePro.csproj` in Visual Studio
2. Build > Build Solution
3. Run the application

**Option 2: Using .NET CLI**
```bash
cd SecureNotePro
dotnet build
dotnet run
```

**Option 3: Build Release Version**
```bash
cd SecureNotePro
dotnet build -c Release
```

The release build will be optimized and have no debug symbols, making it more realistic for RE practice.

### Output Location
- Debug: `SecureNotePro/bin/Debug/net8.0-windows/`
- Release: `SecureNotePro/bin/Release/net8.0-windows/`

## 📖 Protection Analysis Guide

### Step 1: Reconnaissance
1. Run the application and explore features
2. Try premium features to see error messages
3. Monitor file system and registry with Process Monitor
4. Examine the executable with PE tools

### Step 2: Static Analysis
1. Decompile with ILSpy or dnSpy
2. Read the protection code (all documented!)
3. Map out the validation flow
4. Identify key functions and algorithms

### Step 3: Dynamic Analysis
1. Set breakpoints in license validation
2. Trace execution flow
3. Examine variable values
4. Identify bypass points

### Step 4: Exploitation
1. Choose your approach:
   - **Keygen**: Generate valid keys
   - **Patch**: Modify validation logic
   - **Crack**: Remove protection completely
   - **Memory**: Runtime modification
2. Implement your solution
3. Test thoroughly

## 🎓 Learning Path

### Phase 1: Understanding (1-2 weeks)
- Read all source code
- Understand each protection technique
- Document the validation flow
- Try valid license keys

### Phase 2: Analysis (1-2 weeks)
- Use debuggers to trace execution
- Identify critical validation points
- Find decoy functions
- Map protection layers

### Phase 3: Bypassing (2-4 weeks)
- Start with simple patches
- Progress to complex bypasses
- Write a keygen
- Create universal patches

### Phase 4: Mastery (Ongoing)
- Study similar real-world protections
- Learn more advanced techniques
- Practice on other targets
- Contribute to security research

## ⚖️ Legal and Ethical Considerations

### ✅ Acceptable Use
- Educational learning
- Security research
- Penetration testing training
- CTF competitions
- Academic study

### ❌ Prohibited Use
- Cracking commercial software
- Distributing cracks/keygens for paid software
- Bypassing protections for piracy
- Any illegal activities

**Remember:** The skills learned here should be used **ethically and legally**. Always respect intellectual property rights and software licenses in real-world scenarios.

## 🏆 Achievements

Track your progress:

- [ ] Successfully activated with provided license key
- [ ] Located all trial data storage locations
- [ ] Decrypted all obfuscated strings
- [ ] Identified all decoy functions
- [ ] Reset trial period successfully
- [ ] Bypassed anti-debugging checks
- [ ] Wrote a working keygen
- [ ] Patched a single feature
- [ ] Created universal unlock patch
- [ ] Bypassed integrity checks
- [ ] Performed memory-only patching
- [ ] Built a custom loader

## 🤝 Educational Resources

### Recommended Reading
- *Practical Reverse Engineering* by Bruce Dang
- *The Art of Memory Forensics* by Michael Hale Ligh
- *.NET IL Assembler* by Serge Lidin
- *Reversing: Secrets of Reverse Engineering* by Eldad Eilam

### Online Resources
- [OSDev Wiki](https://wiki.osdev.org/)
- [Reverse Engineering Stack Exchange](https://reverseengineering.stackexchange.com/)
- [OpenSecurityTraining](https://opensecuritytraining.info/)
- [Crackmes.one](https://crackmes.one/) - Practice challenges

### Video Tutorials
- LiveOverflow YouTube channel
- MalwareTech tutorials
- OALabs Research

## 📝 Documentation

Each protection class contains extensive inline documentation explaining:
- What the protection does
- How it works
- Why it's implemented that way
- What to look for when analyzing it

Read the source code comments marked with `// PROTECTION:` for detailed insights.

## 🐛 Known "Vulnerabilities" (Intentional)

These are intentionally left for educational discovery:

1. Decoy functions are marked in comments (but you should find them yourself!)
2. Encryption keys are embedded in the binary (realistic scenario)
3. License generation algorithm is included (usually server-side only)
4. Integrity checks can be bypassed with careful patching
5. Trial data locations are documented (simulate research)

## 💡 Hints

<details>
<summary>Hint 1: Quick Feature Unlock</summary>

Look at the `FeatureGate.CheckFeatureAccess()` method. What happens if it returns `true`?
</details>

<details>
<summary>Hint 2: License Validation</summary>

The `ValidateLicense()` method calls multiple checks. Do ALL of them need to pass? Look at the boolean logic carefully.
</details>

<details>
<summary>Hint 3: Trial Reset</summary>

Trial data is stored in 3 locations. You need to modify all 3. Also, the timestamps are obfuscated - check `ObfuscateTimestamp()`.
</details>

<details>
<summary>Hint 4: String Decryption</summary>

The XOR keys rotate through three values. The pattern is in `GetXorKeyForPosition()`.
</details>

<details>
<summary>Hint 5: Keygen</summary>

Study `GenerateLicenseKey()` and `CalculateChecksum()`. The algorithm is deterministic!
</details>

## 🔧 Troubleshooting

### Application won't start
- Ensure .NET 8.0 Runtime is installed
- Check Windows compatibility (Windows 10/11)
- Run as administrator if needed

### Anti-debug warnings appear
- This is normal when debugging - it's part of the protection!
- Try bypassing the checks as part of the exercise

### Can't find trial data
- Use Process Monitor to watch file/registry access
- Check hidden files are visible in Explorer
- Look in the documented locations

## 📞 Support

This is an educational project. For issues or questions:
- Read the source code first (it's documented!)
- Check the hints section
- Review the protection technique explanations
- Try different analysis approaches

## 📜 License

This educational software is provided for learning purposes. The code demonstrates protection techniques and is meant to be analyzed, modified, and broken as part of the learning process.

**MIT License** - Feel free to use, modify, and learn from this code.

---

## 🎯 Final Note

The goal of this project is **education**, not to encourage software piracy. The techniques learned here are valuable for:
- Security researchers
- Malware analysts
- Software developers (to protect their work)
- Penetration testers
- CTF participants

**Use your knowledge responsibly and ethically!**

Happy reverse engineering! 🔍

---

*Last Updated: 2024*
*Version: 1.0.0*

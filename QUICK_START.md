# SecureNote Pro - Quick Start Guide

## 🚀 Run the Application

### Debug Build (with symbols)
```bash
cd "c:\Users\motyl\ADDRESS FILLER\REVERSE-ENGINEER\SecureNotePro"
dotnet run
```

### Release Build (for RE practice - recommended!)
```bash
cd "c:\Users\motyl\ADDRESS FILLER\REVERSE-ENGINEER\SecureNotePro"
dotnet build -c Release
cd bin\Release\net8.0-windows
.\SecureNotePro.exe
```

## 🔑 Test License Keys

Try these valid license keys in the app:

```
SK4N-0G7A-4NDI-4L75
2MBL-7S5X-CNQI-3L8N
XWMQ-A39T-0VKA-0T9I
8IM2-3LA6-WNM2-1L1D
5DYL-8S3Z-XPF1-2T4Y
```

## 🎯 Your Mission

**Goal:** Unlock all premium features without a valid license key!

**Premium Features to Unlock:**
- 🏷️ Tags for notes
- ⭐ Favorite marking
- 📁 Category organization
- 🔍 Advanced search
- 📊 Export to multiple formats

## 🛠️ Recommended Tools

1. **dnSpy** - .NET debugger/decompiler (easiest to start with)
   - Download: https://github.com/dnSpy/dnSpy/releases
   - Open: `bin\Release\net8.0-windows\SecureNotePro.dll`

2. **ILSpy** - .NET decompiler (for reading code)
   - Download: https://github.com/icsharpcode/ILSpy/releases

3. **x64dbg** - Native debugger (advanced)
   - Download: https://x64dbg.com/

## 📚 Protection Layers to Bypass

1. **License Validation** (`Protection/LicenseValidator.cs`)
   - Custom checksum algorithm
   - Multiple validation checks
   - Decoy functions

2. **Anti-Debugging** (`Protection/AntiDebug.cs`)
   - IsDebuggerPresent check
   - Timing-based detection
   - Background monitoring

3. **Code Integrity** (`Protection/IntegrityCheck.cs`)
   - Assembly hash verification
   - Method size checking
   - Patch detection

4. **String Obfuscation** (`Protection/ObfuscatedStrings.cs`)
   - XOR encryption
   - Base64 encoding
   - Runtime decryption

5. **Trial Management** (`Protection/TrialManager.cs`)
   - **2-minute trial** (very short!)
   - Real-time countdown
   - Warning popups
   - Registry storage
   - Hidden files
   - Timestamp obfuscation

6. **Feature Gating** (`Protection/FeatureGate.cs`)
   - Multi-layer validation
   - Runtime checks
   - Access control

## 💡 Beginner Path

### Step 1: Analyze (30 mins)
1. Run the app and explore features
2. Try clicking premium features (they're locked!)
3. Open README.md and read about protections

### Step 2: Decompile (1 hour)
1. Install dnSpy
2. Open `bin\Release\net8.0-windows\SecureNotePro.dll`
3. Browse to `Protection/FeatureGate.cs`
4. Read the `CheckFeatureAccess()` method

### Step 3: Simple Patch (30 mins)
**CHALLENGE:** Make `CheckFeatureAccess()` always return `true`

1. In dnSpy, right-click `CheckFeatureAccess` method
2. Choose "Edit Method (C#)..."
3. Change the method to:
   ```csharp
   public static bool CheckFeatureAccess(string featureName)
   {
       return true; // PATCHED!
   }
   ```
4. Click "Compile"
5. File > Save Module
6. Run the patched app - all features unlocked!

### Step 4: Try Other Approaches
- Generate a valid license key (study `LicenseValidator.cs`)
- Reset the trial period (find trial data locations)
- Bypass anti-debug checks
- Decrypt obfuscated strings

## 🏆 Challenges (Difficulty Progression)

### ⭐ Beginner
- [ ] Activate with a provided license key
- [ ] Find where trial data is stored
- [ ] Patch one premium feature
- [ ] Bypass one anti-debug check

### ⭐⭐ Intermediate
- [ ] Write a keygen to generate valid keys
- [ ] Reset trial to 30 days
- [ ] Patch all anti-debug checks
- [ ] Decrypt all obfuscated strings manually

### ⭐⭐⭐ Advanced
- [ ] Universal patch (one change unlocks everything)
- [ ] Bypass integrity checks
- [ ] Memory-only patching (no file modification)
- [ ] Build a custom loader/injector

## 📖 Full Documentation

For complete information, see:
- [README.md](SecureNotePro/README.md) - Full documentation
- [BUILD.md](BUILD.md) - Build instructions

## ⚠️ Educational Use Only

This software is for learning purposes only. Use your skills ethically and legally!

---

**Ready to start?** Open dnSpy and load `SecureNotePro.dll`! 🔍

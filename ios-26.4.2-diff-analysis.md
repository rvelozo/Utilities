# iOS 26.4.2 (Build 23E261) - Diff Analysis

**Released:** April 22, 2026
**Compared against:** iOS 26.4.1 (Build 23E254)
**Source:** [blacktop/ipsw-diffs](https://github.com/blacktop/ipsw-diffs)

---

## TL;DR - Should You Update?

**YES - Update immediately.** iOS 26.4.2 patches **CVE-2026-28950**, a serious privacy vulnerability where deleted notification content (including messages from Signal, WhatsApp, etc.) was retained on-device and could be forensically recovered. The FBI demonstrated this by extracting deleted Signal messages from a suspect's iPhone. This is a security-critical update.

---

## What Apple Says

- "Bug fixes and security updates"
- Notifications marked for deletion could be unexpectedly retained on the device
- Security content: [support.apple.com/en-us/127002](https://support.apple.com/en-us/127002)

## The Security Vulnerability (CVE-2026-28950)

### What was broken
iOS kept copies of push notification content in an internal notification database — even after messages were deleted, even after the app was uninstalled entirely.

### Real-world impact
The FBI successfully extracted Signal messages from a defendant's iPhone using this flaw. Incoming Signal messages were inadvertently saved in the device's push notification database even after the user deleted Signal.

### Who's affected
Anyone who uses encrypted messaging apps (Signal, WhatsApp, Telegram) or any app that delivers sensitive content via push notifications. The exposure is primarily a **confidentiality** risk.

### The fix
Improved data redaction in notification handling. The system now properly purges notification content from internal databases when notifications are dismissed or apps are deleted.

---

## What Actually Changed (IPSW Firmware Diff)

### Kernel & iBoot
No changes — both remain kernel 25.4.0 (12377.102.10~3), iBoot mBoot-18000.102.4

### WebKit
No changes (624.1.16.10.6)

### Updated Binaries (5 total)

| Binary | Old Version | New Version | What Changed |
|--------|-------------|-------------|--------------|
| **iMessage** | 1450.500.221.2.9 | 1450.500.221.2.14 | Added `_IMSharedHelperPayloadByStrippingServerBagKeys`; new group message payload acceptance logic with known-sender classification |
| **HeuristicInterpreter** | 627.11.0.0.0 | 627.11.0.1.0 | Minor const expansion; removed dependencies on libMobileGestalt, libSystem.B, libobjc.A |
| **AppPredictionIntentsHelperService** | 627.11.0.0.0 | 627.11.0.1.0 | Const expansion; removed LinkMetadata, libSystem.B, libobjc.A dependencies |
| **ActionPredictionNotifications** | 627.11.0.0.0 | 627.11.0.1.0 | Const section expansion |
| **duetexpertd** | 627.11.0.0.0 | 627.11.0.1.0 | Const section expansion |

### Updated Dynamic Libraries (10 total)

| Framework | Key Changes |
|-----------|-------------|
| **AppPredictionClient** | Replaced string-based notification fields with length-based approach (`bodyLength`, `subtitleLength`, `titleLength`). Protobuf schema updated. 11 new functions, 37 new symbols. |
| **AppPredictionInternal** | **Critical:** Added `_purgeNotificationBiomeStreamsIfNeeded` — "Purging private notification streams to remove persisted text content" |
| **AppPredictionFoundation** | Added `__kATXBiomeNotificationPurgeCompleteKey` |
| **IMSharedUtilities** | Added `_IMServerBagValueForKnownSender`, `_IMSharedHelperPayloadByStrippingServerBagKeys`; 7 new C-strings for server bag handling and sender classification |
| **PosterFuturesKit** | Added `PFTFutureResult` init with lock mechanism (`_lock`, `_lock_error`, `_lock_result`) |
| **PosterLegibilityKit** | Added `isFinished` message send |
| **ActionPredictionHeuristics** | Const expansion |
| **ActionPredictionHeuristicsInternal** | Const expansion |
| **AppPredictionUIFoundation** | Const expansion |
| **VisualActionPredictionCore** | Const expansion |

---

## Key Fixes Explained

### 1. Notification Data Purging (CVE-2026-28950 fix)
- **Core change:** `_purgeNotificationBiomeStreamsIfNeeded` added to AppPredictionInternal
- **What it does:** Actively purges private notification text content from Biome streams (Apple's on-device ML logging system)
- **Completion tracking:** New `__kATXBiomeNotificationPurgeCompleteKey` ensures purge finishes
- **Data minimization:** Notification fields now store only `bodyLength`/`titleLength`/`subtitleLength` instead of full text content

### 2. iMessage Server Bag Payload Stripping
- New function strips server-bag keys from message payloads before processing
- Added known-sender classification for group message acceptance
- Likely hardens iMessage against metadata leakage

### 3. PosterFuturesKit Thread Safety
- Added proper lock mechanism to `PFTFutureResult`
- Fixes potential race condition in Lock Screen poster rendering

---

## Security

| CVE | Severity | Description |
|-----|----------|-------------|
| **CVE-2026-28950** | High (Privacy) | Notification data retention flaw allowed recovery of deleted message content. Exploited in law enforcement forensics. |

Also patched in iOS 18.7.8 for older devices.

---

## Verdict

**This is a must-install update**, especially if you:
- Use Signal, WhatsApp, Telegram, or any encrypted messaging app
- Handle sensitive information via notifications
- Care about data being properly deleted when you delete it
- Are in a profession where device forensics is a concern (journalism, law, activism)

There is no good reason to skip this update. The risk is purely on the privacy/confidentiality side, but it's a serious one given real-world forensic exploitation has been demonstrated.

---

## Timeline

- **iOS 26.4** (March 18) — Major release, 35+ security fixes
- **iOS 26.4.1** (April 8) — iCloud/CloudKit sync bug fix, no security patches
- **iOS 26.4.2** (April 22) — Critical notification privacy fix (CVE-2026-28950)
- **iOS 26.5** (expected mid-May) — Next feature release, currently in beta 4

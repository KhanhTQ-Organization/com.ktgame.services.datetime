# com.ktgame.services.datetime

DateTime service (`IDateTimeService`) cho KTgame Architecture.

- `Now` / `NowUtc`: giờ NTP (pool.ntp.org). Khi chưa có NTP (offline) dùng UnbiasedTime, không bao giờ lỗi.
- `IsSynchronized`, `Synchronized` (event), `WaitForSynchronizationAsync(timeout)`: biết khi nào đã có giờ NTP.
  Offline thì `WaitForSynchronizationAsync` trả về `false` ngay.
- Editor / define `DEVELOPMENT`: `Now` là giờ máy (để test/cheat). Bỏ `DEVELOPMENT` khi build release.
- Tự đồng bộ lại NTP khi app quay lại từ background.

1. User secret

```powershell
# 進入到
cd {gitDir}\Bookkeeping\Bookkeeping

# 建立使用者的 user-secrets
dotnet user-secrets init --id 647e7628-55a7-4ce9-9cf5-a23436a78f88

# 設定 user-secrets 的 DB 連線資訊
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=;Database=;user id=;password="
dotnet user-secrets set "LineBot:ChannelAccessToken" ""
dotnet user-secrets set "LineBot:ChannelSecret" ""

```

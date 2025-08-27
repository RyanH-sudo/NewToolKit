# Hardware Heroes PowerShell Script Demos
# Cosmic troubleshooting scripts for the Hardware Heroes module

Write-Host "🦸‍♂️ Hardware Heroes PowerShell Toolkit" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan

# Script 1: Meet Your Network Interface Cards
Write-Host "`n🗣️ Script 1: Meet Your Chatty NICs" -ForegroundColor Yellow
Write-Host "-------------------------------------"

function Show-NetworkAdapters {
    Write-Host "Discovering your network interface heroes..." -ForegroundColor Green
    
    $adapters = Get-NetAdapter | Where-Object { $_.Status -eq "Up" }
    
    foreach ($adapter in $adapters) {
        Write-Host "`n💬 NIC Hero: $($adapter.Name)" -ForegroundColor Magenta
        Write-Host "   Status: $($adapter.Status) (Ready for cosmic conversations!)" -ForegroundColor White
        Write-Host "   Speed: $($adapter.LinkSpeed) (Chatting at light speed!)" -ForegroundColor White
        Write-Host "   MAC Address: $($adapter.MacAddress) (Unique cosmic ID)" -ForegroundColor White
        Write-Host "   Interface Description: $($adapter.InterfaceDescription)" -ForegroundColor Gray
        
        # Get IP configuration
        $ipConfig = Get-NetIPAddress -InterfaceIndex $adapter.InterfaceIndex -AddressFamily IPv4 -ErrorAction SilentlyContinue
        if ($ipConfig) {
            Write-Host "   IP Address: $($ipConfig.IPAddress) (Current cosmic location)" -ForegroundColor Cyan
        }
    }
}

Show-NetworkAdapters

# Script 2: Switch Detective Work
Write-Host "`n🎧 Script 2: Switch DJ Status Check" -ForegroundColor Yellow
Write-Host "-------------------------------------"

function Get-SwitchConnections {
    Write-Host "Checking your network party connections..." -ForegroundColor Green
    
    $statistics = Get-NetAdapter | Get-NetAdapterStatistics
    
    foreach ($stat in $statistics) {
        $adapter = Get-NetAdapter -InterfaceIndex $stat.InterfaceIndex -ErrorAction SilentlyContinue
        if ($adapter -and $adapter.Status -eq "Up") {
            Write-Host "`n🎵 DJ Port: $($adapter.Name)" -ForegroundColor Magenta
            Write-Host "   Bytes Sent: $([math]::Round($stat.SentBytes / 1MB, 2)) MB (Party messages sent)" -ForegroundColor Green
            Write-Host "   Bytes Received: $([math]::Round($stat.ReceivedBytes / 1MB, 2)) MB (Party messages received)" -ForegroundColor Green
            Write-Host "   Packets Sent: $($stat.SentUnicastPackets) (Individual conversations)" -ForegroundColor White
            Write-Host "   Packets Received: $($stat.ReceivedUnicastPackets) (Responses heard)" -ForegroundColor White
        }
    }
}

Get-SwitchConnections

# Script 3: Router Traffic Control
Write-Host "`n🚔 Script 3: Router Traffic Director Status" -ForegroundColor Yellow
Write-Host "---------------------------------------------"

function Show-RoutingTable {
    Write-Host "Checking cosmic traffic routes..." -ForegroundColor Green
    
    $routes = Get-NetRoute -AddressFamily IPv4 | Where-Object { $_.RouteMetric -lt 1000 } | Select-Object -First 10
    
    Write-Host "`n🛣️ Top Cosmic Highways:" -ForegroundColor Magenta
    foreach ($route in $routes) {
        $destination = if ($route.DestinationPrefix -eq "0.0.0.0/0") { "🌌 Entire Internet" } else { $route.DestinationPrefix }
        Write-Host "   Route: $destination → Gateway: $($route.NextHop)" -ForegroundColor White
        Write-Host "     Interface: $($route.InterfaceAlias) (Traffic cop on duty)" -ForegroundColor Gray
    }
}

Show-RoutingTable

# Script 4: Hardware Health Check
Write-Host "`n🏥 Script 4: Hardware Heroes Health Check" -ForegroundColor Yellow
Write-Host "-------------------------------------------"

function Test-NetworkConnectivity {
    Write-Host "Checking if your hardware heroes can reach the internet..." -ForegroundColor Green
    
    $testHosts = @(
        @{Name="Google DNS"; Host="8.8.8.8"},
        @{Name="Cloudflare DNS"; Host="1.1.1.1"},
        @{Name="Microsoft"; Host="microsoft.com"}
    )
    
    foreach ($test in $testHosts) {
        Write-Host "`n🔍 Testing connection to $($test.Name)..." -ForegroundColor Cyan
        
        try {
            $result = Test-NetConnection -ComputerName $test.Host -Port 53 -InformationLevel Quiet
            if ($result) {
                Write-Host "   ✅ SUCCESS! Your heroes can reach $($test.Name)" -ForegroundColor Green
            } else {
                Write-Host "   ❌ FAILED! Cannot reach $($test.Name)" -ForegroundColor Red
            }
        }
        catch {
            Write-Host "   ⚠️ ERROR: $($_.Exception.Message)" -ForegroundColor Yellow
        }
    }
}

Test-NetworkConnectivity

# Script 5: Firewall Guardian Status
Write-Host "`n🛡️ Script 5: Firewall Guardian Check" -ForegroundColor Yellow
Write-Host "------------------------------------"

function Check-FirewallStatus {
    Write-Host "Checking your guardian firewall status..." -ForegroundColor Green
    
    try {
        $firewallProfiles = Get-NetFirewallProfile
        
        foreach ($profile in $firewallProfiles) {
            $status = if ($profile.Enabled) { "🛡️ PROTECTING" } else { "⚠️ DISABLED" }
            $color = if ($profile.Enabled) { "Green" } else { "Red" }
            
            Write-Host "`n🔥 $($profile.Name) Profile: $status" -ForegroundColor $color
            Write-Host "   Inbound: $($profile.DefaultInboundAction) (Default guardian response)" -ForegroundColor White
            Write-Host "   Outbound: $($profile.DefaultOutboundAction) (Default exit policy)" -ForegroundColor White
        }
    }
    catch {
        Write-Host "   ⚠️ Cannot access firewall information: $($_.Exception.Message)" -ForegroundColor Yellow
    }
}

Check-FirewallStatus

# Script 6: Advanced Hardware Discovery
Write-Host "`n🔬 Script 6: Advanced Hardware Discovery" -ForegroundColor Yellow
Write-Host "---------------------------------------"

function Discover-NetworkHardware {
    Write-Host "Scanning for network hardware heroes in your system..." -ForegroundColor Green
    
    try {
        # Get network adapter hardware info
        $adapters = Get-CimInstance -ClassName Win32_NetworkAdapter | Where-Object { $_.NetEnabled -eq $true }
        
        foreach ($adapter in $adapters) {
            Write-Host "`n🦸‍♂️ Hardware Hero Found: $($adapter.Name)" -ForegroundColor Magenta
            Write-Host "   Manufacturer: $($adapter.Manufacturer) (Hero creator)" -ForegroundColor White
            Write-Host "   MAC Address: $($adapter.MACAddress) (Cosmic fingerprint)" -ForegroundColor Cyan
            Write-Host "   Adapter Type: $($adapter.AdapterType) (Hero specialty)" -ForegroundColor White
            Write-Host "   Status: $($adapter.NetConnectionStatus) (Current mission status)" -ForegroundColor Gray
        }
        
        # Check for wireless capabilities
        Write-Host "`n📡 Wireless Wizard Check:" -ForegroundColor Cyan
        $wirelessAdapters = Get-NetAdapter | Where-Object { $_.PhysicalMediaType -like "*wireless*" -or $_.Name -like "*wi-fi*" -or $_.Name -like "*wireless*" }
        
        if ($wirelessAdapters) {
            foreach ($wireless in $wirelessAdapters) {
                Write-Host "   🧙‍♂️ Wireless Wizard: $($wireless.Name) (Magic signal powers activated!)" -ForegroundColor Magenta
            }
        } else {
            Write-Host "   📡 No wireless wizards detected (Wired heroes only)" -ForegroundColor Gray
        }
        
    }
    catch {
        Write-Host "   ⚠️ Hardware discovery failed: $($_.Exception.Message)" -ForegroundColor Yellow
    }
}

Discover-NetworkHardware

# Script 7: Performance Monitoring
Write-Host "`n⚡ Script 7: Hardware Performance Monitor" -ForegroundColor Yellow
Write-Host "----------------------------------------"

function Monitor-NetworkPerformance {
    Write-Host "Checking your hardware heroes' performance..." -ForegroundColor Green
    
    try {
        $perfCounters = @(
            "\Network Interface(*)\Bytes Total/sec",
            "\Network Interface(*)\Packets/sec",
            "\Network Interface(*)\Current Bandwidth"
        )
        
        $activeAdapters = Get-NetAdapter | Where-Object { $_.Status -eq "Up" -and $_.Virtual -eq $false }
        
        foreach ($adapter in $activeAdapters) {
            Write-Host "`n🏃‍♂️ Performance Check: $($adapter.Name)" -ForegroundColor Magenta
            
            # Get current utilization
            $stats = Get-NetAdapterStatistics -Name $adapter.Name
            Write-Host "   Total Bytes: $([math]::Round(($stats.SentBytes + $stats.ReceivedBytes) / 1GB, 2)) GB (Lifetime conversations)" -ForegroundColor Cyan
            Write-Host "   Link Speed: $($adapter.LinkSpeed) (Maximum hero speed)" -ForegroundColor White
            
            # Calculate basic utilization (simplified)
            $utilizationPercent = if ($adapter.LinkSpeed -match "(\d+)") {
                $linkSpeedBits = [long]$matches[1] * 1000000  # Convert Mbps to bps
                $currentBytes = ($stats.SentBytes + $stats.ReceivedBytes)
                if ($linkSpeedBits -gt 0) {
                    [math]::Round(($currentBytes * 8 / $linkSpeedBits) * 100, 2)
                } else { 0 }
            } else { 0 }
            
            Write-Host "   Estimated Utilization: $utilizationPercent% (Hero workload)" -ForegroundColor $(if ($utilizationPercent -gt 80) { "Red" } elseif ($utilizationPercent -gt 50) { "Yellow" } else { "Green" })
        }
    }
    catch {
        Write-Host "   ⚠️ Performance monitoring failed: $($_.Exception.Message)" -ForegroundColor Yellow
    }
}

Monitor-NetworkPerformance

Write-Host "`n🌟 Hardware Heroes PowerShell Toolkit Complete!" -ForegroundColor Cyan
Write-Host "Your network hardware heroes are ready for action!" -ForegroundColor Green
Write-Host "`nRemember: Every great network engineer started by understanding their hardware heroes! 🦸‍♂️✨" -ForegroundColor Magenta
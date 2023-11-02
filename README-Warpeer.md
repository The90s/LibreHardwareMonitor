## 库包装说明

### 兼容

现在只用.net framework 4.7.2 版本编译出来的库：dll 文件在`bin/*/net472`目录

### 增加的文件及目录:

-   `reports目录`：一些机器的 report 数据
-   `warpper目录`：库包装类
-   `Program.cs`：库暴漏的接口文件
-   `build.sh`：项目构建脚本
-   `PRINT_UPDATE_DATE宏`：用来控制打印输出

### 特性

-   库初始化时间 3 秒 稍长：21:59:48.104 -> 21:59:50 [debug] [main] LibreHardwareMonitorInited: true
-   后面获取性能很好

## TODO

-   [x] AMD 显卡测试
-   [x] 显卡上风扇速度（1/2/3 个风扇）：获取一个,同一个显卡的风扇转速可能差不多
-   [x] 主板上风扇速度：简单处理
-   [x] 加锁：js --> C# Runtime(线程池)
-   [ ] 使用 SMBios 信息
-   [ ] 用这个库代替 systeminfomation（Nodejs 库）
-   [ ] dynamic 类型：编译问题，依赖库问题
-   [ ] 方便调试 libreHardwareMonitor 库的接口：C#回调到 js
    -   [x] 日志控制：输出到stdout
    -   [ ] 输出到文件

## report 文件分析

### 判断项：

-   sensor 的 Name
-   sensor 的 SensorType
-   sensor 的 Value:正常就一个 Value，有些会有多个 Sensor 一样，就会有多个 Value
    -   有些就 0 表示无效
    -   有些有多个大于零的数
    -   有些 Value 就是 null；比如：ZHITAI TiPlus5000 512GB SSD 使用率获取不到

### Misc:

-   Intel 集显温度和风扇就 CPU 的
-   Intel 集显负载：D3D Video Decod 稍微合适一点
-   网络速度：是单个网卡的速度还是多个网卡之和
    -   正常只有一个网卡有速度
    -   虚拟网卡：多个网卡速度加起来之和
-   编译出来的 Debug、Release 版本文件大小一样：感觉是项目配置文件的问题
    -   查看脚本：`ls -hal bin/*/net472`

### AMD Ryzen 9 7950X 1 (/amdcpu/0)

```log
|  +- CCDs Average (Tdie) :  54.0625   51.875    68.75 (/amdcpu/0/temperature/10)
|  +- CPU Total      :  1.44014 0.909638  4.18656 (/amdcpu/0/load/0)
```

### 13th Gen Intel Core i5-13400 (/intelcpu/0)

```log
|  +- Core Average   :     35.4     32.5     55.3 (/intelcpu/0/temperature/22)
|  +- CPU Total      :  5.16657        0  73.1929 (/intelcpu/0/load/0)
```

> Note: AMD 和 Intel 的负载 Name 不一样：CCDs Average (Tdie) --- Core Average

### NVIDIA GeForce RTX 2070 (/gpu-nvidia/0)

```log
|  +- GPU Core       :       37       37       40 (/gpu-nvidia/0/temperature/0)
|  +- GPU Core       :       16        2       19 (/gpu-nvidia/0/load/0)
|  +- GPU Fan 1      :     1068     1059     1074 (/gpu-nvidia/0/fan/1)
|  +- GPU Fan 2      :     1207     1197     1214 (/gpu-nvidia/0/fan/2)
```

### Intel(R) UHD Graphics 730 (/gpu-intel-integrated/xxxxxx)

集显温度和风扇就 CPU 的
负载：D3D Video Decod 稍微合适一点

```log
|  +- D3D 3D         : 0.939093 0.104752  35.2802 (/gpu-intel-integrated/xxxxxx/load/0)
|  +- D3D Copy       :        0        0        0 (/gpu-intel-integrated/xxxxxx/load/1)
|  +- D3D GDI Render :        0        0        0 (/gpu-intel-integrated/xxxxxx/load/2)
|  +- D3D Other      :        0        0        0 (/gpu-intel-integrated/xxxxxx/load/3)
|  +- D3D Other      :        0        0        0 (/gpu-intel-integrated/xxxxxx/load/4)
|  +- D3D Other      :        0        0        0 (/gpu-intel-integrated/xxxxxx/load/5)
|  +- D3D Other      :        0        0        0 (/gpu-intel-integrated/xxxxxx/load/6)
|  +- D3D Other      :        0        0        0 (/gpu-intel-integrated/xxxxxx/load/7)
|  +- D3D Other      :        0        0        0 (/gpu-intel-integrated/xxxxxx/load/8)
|  +- D3D Video Decode :  1.67533        0  13.7776 (/gpu-intel-integrated/xxxxxx/load/9)   xxxxx
|  +- D3D Video Decode :        0        0        0 (/gpu-intel-integrated/xxxxxx/load/10)
|  +- D3D Video Processing :        0        0        0 (/gpu-intel-integrated/xxxxxx/load/11)
|  +- D3D Video Processing :        0        0        0 (/gpu-intel-integrated/xxxxxx/load/12)
```

### Generic Memory (/ram)

```log
|  +- Memory         :  18.1024  18.1024  18.3493 (/ram/load/0)
```

### Network

```log
+- WLAN (/nic/{CAA8A191-A835-453F-87A6-0F3810967B60})
|  +- Upload Speed   :  20105.9  16327.1   801648 (/nic/{CAA8A191-A835-453F-87A6-0F3810967B60}/throughput/7)
|  +- Download Speed :  8235.53  4617.69  80930.7 (/nic/{CAA8A191-A835-453F-87A6-0F3810967B60}/throughput/8)
|
+- 以太网 (/nic/{F5C7D858-2107-4B9B-B822-20B0E35CAB1D})
|  +- Upload Speed   :        0        0        0 (/nic/{F5C7D858-2107-4B9B-B822-20B0E35CAB1D}/throughput/7)
|  +- Download Speed :        0        0        0 (/nic/{F5C7D858-2107-4B9B-B822-20B0E35CAB1D}/throughput/8)
```

## 已知 BUG

-   NVIDIA GeForce RTX 2070 三个风扇只能获取到两个风扇的转速
-   Radeon RX 580 Series 获取不到显卡风扇转速（1 个风扇）
-   ZHITAI TiPlus5000 512GB 使用率获取不到 `Used Space     :                            (/nvme/1/load/0)`

## dotnet

```bash
$ dotnet add package Microsoft.CSharp --version 4.5.0
# 指定项目
$ dotnet add LibreHardwareMonitorLib/LibreHardwareMonitorLib.csproj package Microsoft.CSharp --version 4.7.0
```

## 性能

测试电脑 Intel I5 13400

```log
LibreHardwareMonitor 14:15:34.664 [Debug]: updateStatus start
LibreHardwareMonitor 14:15:34.664 [Debug]: updateStatus start: ASUS PRIME B760M-K D4
LibreHardwareMonitor 14:15:34.664 [Debug]: updateStatus end: ASUS PRIME B760M-K D4
LibreHardwareMonitor 14:15:34.664 [Debug]: CPU Fan: 2242
LibreHardwareMonitor 14:15:34.664 [Debug]: updateStatus start: 13th Gen Intel Core i5-13400
LibreHardwareMonitor 14:15:34.820 [Debug]: updateStatus end: 13th Gen Intel Core i5-13400
LibreHardwareMonitor 14:15:34.820 [Debug]: cpu load: 7
LibreHardwareMonitor 14:15:34.821 [Debug]: cpu temperature: 41
LibreHardwareMonitor 14:15:34.821 [Debug]: updateStatus start: Generic Memory
LibreHardwareMonitor 14:15:34.821 [Debug]: updateStatus end: Generic Memory
LibreHardwareMonitor 14:15:34.822 [Debug]: mem load: 68
LibreHardwareMonitor 14:15:34.822 [Debug]: updateStatus start: AMD Radeon RX 5600 XT
LibreHardwareMonitor 14:15:34.825 [Debug]: updateStatus end: AMD Radeon RX 5600 XT
LibreHardwareMonitor 14:15:34.825 [Debug]: updateStatus start: Colorful CN600 512GB DDR
LibreHardwareMonitor 14:15:34.825 [Debug]: updateStatus end: Colorful CN600 512GB DDR
LibreHardwareMonitor 14:15:34.825 [Debug]: updateStatus start: 以太网
LibreHardwareMonitor 14:15:34.826 [Debug]: updateStatus end: 以太网
LibreHardwareMonitor 14:15:34.826 [Debug]: Update NetWork: upload: 0.390483
LibreHardwareMonitor 14:15:34.826 [Debug]: Update NetWork: download: 2.288502
LibreHardwareMonitor 14:15:34.826 [Debug]: AMD GPU temperature: 44
LibreHardwareMonitor 14:15:34.826 [Debug]: AMD GPU load: 3
LibreHardwareMonitor 14:15:34.827 [Debug]: GPU temperature: 44
LibreHardwareMonitor 14:15:34.827 [Debug]: GPU Fan: 0
LibreHardwareMonitor 14:15:34.827 [Debug]: updateStatus ended
```

**结论：**

-   更新一次数据花费 163ms（827-664）
-   更新 CPU 的时间稍长 156ms

## 库包装说明

### 兼容

现在只用.net framework 4.7.2版本编译出来的库：dll文件在`bin/*/net472`目录

### 增加的文件及目录:

- `reports目录`：一些机器的report数据
- `warpper目录`：库包装类
- `Program.cs`：库暴漏的接口文件
- `build.sh`：项目构建脚本
- `PRINT_UPDATE_DATE宏`：用来控制打印输出

### 特性

- 库初始化时间3秒 稍长：21:59:48.104 -> 21:59:50 [debug] [main] LibreHardwareMonitorInited:  true
- 后面获取性能很好

## TODO

- [ ] AMD显卡测试
- [ ] 显卡上风扇速度（1/2/3个风扇）
- [ ] 主板上风扇速度
- [ ] 用这个库代替systeminfomation（Nodejs库）
- [ ] 加锁：js --> C# Runtime(线程池)
- [ ] 使用SMBios信息

## report文件分析

### 判断项：

- sensor的Name
- sensor的SensorType
- sensor的Value:正常就一个Value，有些会有多个Sensor一样，就会有多个Value
  - 有些就0表示无效
  - 有些有多个大于零的数
  - 有些Value就是null；比如：ZHITAI TiPlus5000 512GB SSD 使用率获取不到

### Misc:

- Intel集显温度和风扇就CPU的
- Intel集显负载：D3D Video Decod 稍微合适一点
- 网络速度：是单个网卡的速度还是多个网卡之和
  - 正常只有一个网卡有速度
  - 虚拟网卡：多个网卡速度加起来之和
- 编译出来的Debug、Release版本文件大小一样：感觉是项目配置文件的问题
  - 查看脚本：`ls -hal bin/*/net472`

### AMD Ryzen 9 7950X 1 (/amdcpu/0)

|  +- CCDs Average (Tdie) :  54.0625   51.875    68.75 (/amdcpu/0/temperature/10)
|  +- CPU Total      :  1.44014 0.909638  4.18656 (/amdcpu/0/load/0)

### 13th Gen Intel Core i5-13400 (/intelcpu/0)

|  +- Core Average   :     35.4     32.5     55.3 (/intelcpu/0/temperature/22)
|  +- CPU Total      :  5.16657        0  73.1929 (/intelcpu/0/load/0)

> Note: AMD和Intel的负载 Name不一样：CCDs Average (Tdie) --- Core Average

### NVIDIA GeForce RTX 2070 (/gpu-nvidia/0)

|  +- GPU Core       :       37       37       40 (/gpu-nvidia/0/temperature/0)
|  +- GPU Core       :       16        2       19 (/gpu-nvidia/0/load/0)
|  +- GPU Fan 1      :     1068     1059     1074 (/gpu-nvidia/0/fan/1)
|  +- GPU Fan 2      :     1207     1197     1214 (/gpu-nvidia/0/fan/2)

### Intel(R) UHD Graphics 730 (/gpu-intel-integrated/xxxxxx)

集显温度和风扇就CPU的
负载：D3D Video Decod 稍微合适一点

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

### Generic Memory (/ram)
|  +- Memory         :  18.1024  18.1024  18.3493 (/ram/load/0)

### Network

+- WLAN (/nic/{CAA8A191-A835-453F-87A6-0F3810967B60})
|  +- Upload Speed   :  20105.9  16327.1   801648 (/nic/{CAA8A191-A835-453F-87A6-0F3810967B60}/throughput/7)
|  +- Download Speed :  8235.53  4617.69  80930.7 (/nic/{CAA8A191-A835-453F-87A6-0F3810967B60}/throughput/8)
|
+- 以太网 (/nic/{F5C7D858-2107-4B9B-B822-20B0E35CAB1D})
|  +- Upload Speed   :        0        0        0 (/nic/{F5C7D858-2107-4B9B-B822-20B0E35CAB1D}/throughput/7)
|  +- Download Speed :        0        0        0 (/nic/{F5C7D858-2107-4B9B-B822-20B0E35CAB1D}/throughput/8)

## 已知BUG

- NVIDIA GeForce RTX 2070 三个风扇只能获取到两个风扇的转速
- Radeon RX 580 Series 获取不到显卡风扇转速（1个风扇）
- ZHITAI TiPlus5000 512GB 使用率获取不到 `Used Space     :                            (/nvme/1/load/0)`

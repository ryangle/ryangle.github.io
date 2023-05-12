# OpenCV

## 安装和配置

1. 下载opencv，解压
2. 配置系统环境变量path：opencv\build\x64\vc16\bin
3. 配置VC++项目属性，VC++目录，包含目录opencv\build\include\opencv2
4. 配置库目录opencv\build\x64\vc16\lib
5. 配置链接器，输入，附加依赖项opencv_world470d.lib（debug项目）

## 扩展模块的安装和配置

1. 安装CMake,https://cmake.org/files
2. 使用CMake编译OpenCV
3. 下载opencv_contrib
4. 构建时配置勾选BUILD_opencv_world、OPENCV_ENABLE_NONFREE、OPENCV_EXTRA_MODULES_PATH
5. 构建后，查看CMakeDownloadLog.txt，查看缺少的文件
6. 使用vs打开OpenCV.sln，在CMakeTarget下的INSTALL项目右键，仅用于项目->仅生成INSTALL

## 运行示例程序


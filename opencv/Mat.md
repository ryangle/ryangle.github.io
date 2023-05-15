# Mat类

1. 什么是Mat类

cv:Mat<_Tp>

2. 创建

cv::Mat::Mat(int rows,int cols,int type)

type:矩阵中存储的数据类型。CV_8UC1、CV_64FC4等。n来构建多通道矩阵。

利用已有Mat类创建新的Mat类

3. 赋值

类方法赋值 eye、diag、ones、zeros

枚举赋值
如： cv::Mat a = (cv::Mat_<int>(3,3)<<1,2,3,4,5,6,7,8,9);

4. 数据的读取




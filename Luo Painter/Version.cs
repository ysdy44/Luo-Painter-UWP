﻿using Luo_Painter.Strings;
using System;

namespace Luo_Painter
{
    public enum VersionType
    {
        /*
        1.3.0
        2025/02/06
        1. 十六进制颜色选择器支持三位数
        2. 修复点击笔刷选择器导致崩溃的问题
        3. 修复重命名项目时的图标错误
        4. 修复界面上的停靠按钮被应用栏挡住的问题
        5. 修复应用程序无法使用明亮主题的问题
        6. 应用支持崩溃时发送异常报告
        7. 修复在自由变换时，数字选择器不工作的问题
        8. 支持导入图像
        9. 允许用户禁止触摸操作（仅使用鼠标或者数位板）
        */
        V130,

        /*
        1.2.0
        2023/10/29
        1. 界面支持从右到左的布局（适用于阿拉伯语）
        */
        V120,

        /*
        1.0.3
        2023/3/23
        1. 修复打开空项目导致崩溃的问题
        2. 修复绘画导致崩溃问题（绘画线程找不到图层对象）
        */
        V103,

        /*
        1.0.2
        2023/3/21
        1. 修复绘画导致崩溃问题（绘画时线程停顿）
        */
        V102,

        /*
        1.0.1
        2023/3/21
        1. 修复显示界面导致崩溃问题（因为数值类型不是整型导致界面崩溃）
        */
        V101,

        /*
        1.0.0
        2023/3/17
        1. 应用程序上架应用商店
        */
        V100,
    }

    public class Version
    {
        public string VersionTitle { get; set; }
        public DateTime UpdateTime { get; set; }
        public string Description { get; set; }
        public string Time => this.UpdateTime.ToShortDateString();

        public static Version Create(VersionType type)
        {
            switch (type)
            {
                case VersionType.V130:
                    return new Version
                    {
                        VersionTitle = "1.3.0",
                        UpdateTime = new DateTime(2025, 2, 6),
                        Description = type.GetString()
                    };
                case VersionType.V120:
                    return new Version
                    {
                        VersionTitle = "1.2.0",
                        UpdateTime = new DateTime(2023, 10, 29),
                        Description = type.GetString()
                    };
                case VersionType.V103:
                    return new Version
                    {
                        VersionTitle = "1.0.3",
                        UpdateTime = new DateTime(2023, 3, 23),
                        Description = type.GetString()
                    };
                case VersionType.V102:
                    return new Version
                    {
                        VersionTitle = "1.0.2",
                        UpdateTime = new DateTime(2023, 3, 21),
                        Description = type.GetString()
                    };
                case VersionType.V101:
                    return new Version
                    {
                        VersionTitle = "1.0.1",
                        UpdateTime = new DateTime(2023, 3, 21),
                        Description = type.GetString()
                    };
                case VersionType.V100:
                    return new Version
                    {
                        VersionTitle = "1.0.0",
                        UpdateTime = new DateTime(2023, 3, 17),
                        Description = type.GetString()
                    };
                default:
                    return new Version
                    {
                        VersionTitle = "0.0.0",
                        UpdateTime = DateTime.MinValue,
                        Description = type.ToString()
                    };
            }
        }
    }
}
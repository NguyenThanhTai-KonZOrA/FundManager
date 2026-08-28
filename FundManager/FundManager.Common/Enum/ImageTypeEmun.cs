using System.ComponentModel;

namespace FundManager.Common.Enum
{
    public enum ImageTypeEnum : byte
    {
        [Description("Outlet Image")]
        Outlet = 0,
        [Description("Slider Image")]
        Slider = 1,
        [Description("Logo Image")]
        Logo = 2,
        [Description("Background Image")]
        Background = 3,
        [Description("Icon Image")]
        Icon = 4,
        [Description("Other Image")]
        Other = 5,
        /// <summary>Slider carousel for hotel / property-wide slideshow.</summary>
        [Description("Slider - Hotel")]
        SliderHotel = 6,
        /// <summary>Slider carousel scoped to a specific outlet.</summary>
        [Description("Slider - Outlet")]
        SliderOutlet = 7
    }
}
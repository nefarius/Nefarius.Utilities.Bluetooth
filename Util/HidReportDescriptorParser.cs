namespace Nefarius.Utilities.Bluetooth.Util;

public class HidReportDescriptorParser
{
    private const int TAG_OFFSET = 4;
    private const int TYPE_OFFSET = 2;
    private const int SIZE_OFFSET = 0;

    private const int TYPE_MASK = 0x03 << TYPE_OFFSET;

    private const int TAG_MASK = (0x0F << TAG_OFFSET) | TYPE_MASK;

    /* size(2bit): 00 - 0B, 01 - 1B, 10 - 2B, 11 - 4B */
    private const int SIZE_MASK = 0x03 << SIZE_OFFSET;

    /* Short Item Size
    */
    private const int Size_0B = 0;
    private const int Size_1B = 1;
    private const int Size_2B = 2;
    private const int Size_4B = 3;

    /* Short Item Type
    */
    private const int MAIN_ITEM = 0x00 << TYPE_OFFSET;
    private const int GLOBAL_ITEM = 0x01 << TYPE_OFFSET;
    private const int LOCAL_ITEM = 0x02 << TYPE_OFFSET;

    /* Input, Output and Feature Items(1B) Data bit
    */
    private const int Data = 0;
    private const int Constant = 1;
    private const int Array = 0;
    private const int Variable = 1 << 1;
    private const int Absolute = 0;
    private const int Relative = 1 << 2;
    private const int No_Wrap = 0;
    private const int Wrap = 1 << 3;
    private const int Linear = 0;
    private const int NonLinear = 1 << 4;
    private const int Preferred_State = 0;
    private const int No_Prefered = 1 << 5;
    private const int No_Null_Position = 0;
    private const int Null_State = 1 << 6;
    private const int Nonvolatile = 0;
    private const int Volatile = 1 << 7;

    /* Collection, End Collection Items Data Byte
    */
    private const int Col_Physical = 0x00;
    private const int Col_Application = 0x01;
    private const int Col_Logical = 0x02;
    private const int Col_Report = 0x03;
    private const int Col_Named_Array = 0x04;
    private const int Col_Usage_Switch = 0x05;
    private const int Col_Usage_Modifier = 0x06;

    private const int System_None = 0;
    private const int System_SI_Linear = 1;
    private const int System_SI_Rotation = 2;
    private const int System_English_Linear = 3;
    private const int System_English_Rotation = 4;

    /* Usage Page Table
    */
    //Undefined 0x00U
    private const int UP_Generic_Desktop = 0x01;
    private const int UP_Simulation_Controls = 0x02;
    private const int UP_VR_Controls = 0x03;
    private const int UP_Sport_Controls = 0x04;
    private const int UP_Game_Controls = 0x05;
    private const int UP_Generic_Device_Controls = 0x06;
    private const int UP_Keyboard_or_Keypad = 0x07;
    private const int UP_LEDs = 0x08;
    private const int UP_Button = 0x09;
    private const int UP_Ordinal = 0x0A;
    private const int UP_Telephony = 0x0B;
    private const int UP_Consumer = 0x0C;

    private const int UP_Digitizer = 0x0D;

    //Reversed 0x0EU
    private const int UP_PID_Page = 0x0F;

    private const int UP_Unicode = 0x10;

    //Reversed 0x11U~0x13U
    private const int UP_Alphanumeric_Display = 0x14;

    //Reversed 0x15U~0x3FU
    private const int UP_Medical_Instruments = 0x40;

    //Reversed 0x41U~0x7FU
    private const int UP_Monitor_pages_1 = 0x80;
    private const int UP_Monitor_pages_2 = 0x81;
    private const int UP_Monitor_pages_3 = 0x82;
    private const int UP_Monitor_pages_4 = 0x83;
    private const int UP_Power_pages_1 = 0x84;
    private const int UP_Power_pages_2 = 0x85;
    private const int UP_Power_pages_3 = 0x86;

    private const int UP_Power_pages_4 = 0x87;

    //Reversed 0x88U~0x8BU
    private const int UP_Bar_Code_Scanner_page = 0x8C;
    private const int UP_Scale_page = 0x8D;

    private const int UP_MSR_Devices = 0x8E; /* Magnetic Stripe Reading */

    //Reversed point for sale pages 0x8FU
    private const int UP_Camera_Control_Page = 0x90;
    private const int UP_Arcade_Page = 0x91;

    /* Main Item Tag
    */
    private static int Input(int size)
    {
        return MAIN_ITEM | (0x08 << TAG_OFFSET) | ((byte)size & SIZE_MASK);
    }

    private static int Output(int size)
    {
        return MAIN_ITEM | (0x09 << TAG_OFFSET) | ((byte)size & SIZE_MASK);
    }

    private static int Feature(int size)
    {
        return MAIN_ITEM | (0x0B << TAG_OFFSET) | ((byte)size & SIZE_MASK);
    }

    private static int Collection(int size)
    {
        return MAIN_ITEM | (0x0A << TAG_OFFSET) | ((byte)size & SIZE_MASK);
    }

    private static int End_Collection(int size)
    {
        return MAIN_ITEM | (0x0C << TAG_OFFSET) | ((byte)size & SIZE_MASK);
    }

    /* Global Item Tag
    */
    private static int Usage_Page(int size)
    {
        return GLOBAL_ITEM | (0x00 << TAG_OFFSET) | ((byte)size & SIZE_MASK);
    }

    private static int Logical_Minimum(int size)
    {
        return GLOBAL_ITEM | (0x01 << TAG_OFFSET) | ((byte)size & SIZE_MASK);
    }

    private static int Logical_Maximum(int size)
    {
        return GLOBAL_ITEM | (0x02 << TAG_OFFSET) | ((byte)size & SIZE_MASK);
    }

    private static int Physical_Minimum(int size)
    {
        return GLOBAL_ITEM | (0x03 << TAG_OFFSET) | ((byte)size & SIZE_MASK);
    }

    private static int Physical_Maximum(int size)
    {
        return GLOBAL_ITEM | (0x04 << TAG_OFFSET) | ((byte)size & SIZE_MASK);
    }

    /* Unit Exponent: |   Code   | 0x5 | 0x6 | 0x7 | 0x8 | 0x9 | 0xA | 0xB | 0xC | 0xD | 0xE | 0xF |
    **                | Exponent |  5  |  6  |  7  | -8  | -7  | -6  | -5  | -4  | -3  | -2  | -1  |
    */
    private static int Unit_Exponent(int size)
    {
        return GLOBAL_ITEM | (0x05 << TAG_OFFSET) | ((byte)size & SIZE_MASK);
    }

    /* Unit: 按半字节(Nibble)拆分，最低位半字节声明使用的计量系统，其余每个半字节声明对应单位的指数(-7~7)
    **  | Nibble |   0    |    1   |  2   |  3   |      4      |    5    |         6          |    7     |
    **  | Parts  | System | Length | Mass | Time | Temperature | Current | Luminous intensity | Reversed |
    **
    **  | Value  | 0x0  |    0x1    |     0x2     |      0x3       |        0x4       |  Other   |
    **  | System | None | SI Linear | SI Rotation | English Linear | English Rotation | Reversed |
    */
    private static int Unit(int size)
    {
        return GLOBAL_ITEM | (0x06 << TAG_OFFSET) | ((byte)size & SIZE_MASK);
    }

    private static int Report_Size(int size)
    {
        return GLOBAL_ITEM | (0x07 << TAG_OFFSET) | ((byte)size & SIZE_MASK);
    }

    private static int Report_ID(int size)
    {
        return GLOBAL_ITEM | (0x08 << TAG_OFFSET) | ((byte)size & SIZE_MASK);
    }

    private static int Report_Count(int size)
    {
        return GLOBAL_ITEM | (0x09 << TAG_OFFSET) | ((byte)size & SIZE_MASK);
    }

    private static int Push(int size)
    {
        return GLOBAL_ITEM | (0x0A << TAG_OFFSET) | ((byte)size & SIZE_MASK);
    }

    private static int Pop(int size)
    {
        return GLOBAL_ITEM | (0x0B << TAG_OFFSET) | ((byte)size & SIZE_MASK);
    }

/* Local Item Tag
*/
    private static int Usage(int size)
    {
        return LOCAL_ITEM | (0x00 << TAG_OFFSET) | ((byte)size & SIZE_MASK);
    }

    private static int Usage_Minimum(int size)
    {
        return LOCAL_ITEM | (0x01 << TAG_OFFSET) | ((byte)size & SIZE_MASK);
    }

    private static int Usage_Maximum(int size)
    {
        return LOCAL_ITEM | (0x02 << TAG_OFFSET) | ((byte)size & SIZE_MASK);
    }

    private static int Designator_Index(int size)
    {
        return LOCAL_ITEM | (0x03 << TAG_OFFSET) | ((byte)size & SIZE_MASK);
    }

    private static int Designator_Minimum(int size)
    {
        return LOCAL_ITEM | (0x04 << TAG_OFFSET) | ((byte)size & SIZE_MASK);
    }

    private static int Designator_Maximum(int size)
    {
        return LOCAL_ITEM | (0x05 << TAG_OFFSET) | ((byte)size & SIZE_MASK);
    }

    private static int String_Index(int size)
    {
        return LOCAL_ITEM | (0x07 << TAG_OFFSET) | ((byte)size & SIZE_MASK);
    }

    private static int String_Minimum(int size)
    {
        return LOCAL_ITEM | (0x08 << TAG_OFFSET) | ((byte)size & SIZE_MASK);
    }

    private static int String_Maximum(int size)
    {
        return LOCAL_ITEM | (0x09 << TAG_OFFSET) | ((byte)size & SIZE_MASK);
    }

    private static int Delimiter(int size)
    {
        return LOCAL_ITEM | (0x0A << TAG_OFFSET) | ((byte)size & SIZE_MASK);
    }
    //Reversed 0x92U~0xFEFFU
    //Vendor-defined 0xFF00U~0xFFFFU
}
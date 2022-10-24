namespace Nefarius.Utilities.Bluetooth.Util;

/// <summary>
///     HID Report Descriptor parser.
/// </summary>
/// <remarks>Source: https://github.com/uint32tMnstr/USB-HID-Report-Parser</remarks>
public static class HidReportDescriptorParser
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

    #region Usages

    /* Usage Page: Generic Desktop (= 0x01U)
    ** Sys: System
    */
    private const int GD_Pointer = 0x01;

    private const int GD_Mouse = 0x02;

    /* Reserved */
    private const int GD_Joystick = 0x04;
    private const int GD_Game_Pad = 0x05;
    private const int GD_Keyboard = 0x06;
    private const int GD_Keypad = 0x07;
    private const int GD_Multiaxis_Controller = 0x08;

    private const int GD_Tablet_PC_Sys_Controls = 0x09;

    /* Reserved */
    private const int GD_X = 0x30;
    private const int GD_Y = 0x31;
    private const int GD_Z = 0x32;
    private const int GD_Rx = 0x33;
    private const int GD_Ry = 0x34;
    private const int GD_Rz = 0x35;
    private const int GD_Slider = 0x36;
    private const int GD_Dial = 0x37;
    private const int GD_Wheel = 0x38;
    private const int GD_Hat_Switch = 0x39;
    private const int GD_Counted_Buffer = 0x3A;
    private const int GD_Byte_Count = 0x3B;
    private const int GD_Motion_Wakeup = 0x3C;
    private const int GD_Start = 0x3D;

    private const int GD_Select = 0x3E;

    /* Reserved */
    private const int GD_Vx = 0x40;
    private const int GD_Vy = 0x41;
    private const int GD_Vz = 0x42;
    private const int GD_Vbrx = 0x43;
    private const int GD_Vbry = 0x44;
    private const int GD_Vbrz = 0x45;
    private const int GD_Vno = 0x46;
    private const int GD_Feature_Notification = 0x47;

    private const int GD_Resolution_Multiplier = 0x48;

    /* Reserved */
    private const int GD_Sys_Control = 0x80;
    private const int GD_Sys_Power_Down = 0x81;
    private const int GD_Sys_Sleep = 0x82;
    private const int GD_Sys_Wake_Up = 0x83;
    private const int GD_Sys_Context_Menu = 0x84;
    private const int GD_Sys_Main_Menu = 0x85;
    private const int GD_Sys_App_Menu = 0x86;
    private const int GD_Sys_Menu_Help = 0x87;
    private const int GD_Sys_Menu_Exit = 0x88;
    private const int GD_Sys_Menu_Select = 0x89;
    private const int GD_Sys_Menu_Right = 0x8A;
    private const int GD_Sys_Menu_Left = 0x8B;
    private const int GD_Sys_Menu_Up = 0x8C;
    private const int GD_Sys_Menu_Down = 0x8D;
    private const int GD_Sys_Cold_Restart = 0x8E;
    private const int GD_Sys_Warm_Restart = 0x8F;
    private const int GD_D_pad_Up = 0x90;
    private const int GD_D_pad_Down = 0x91;
    private const int GD_D_pad_Right = 0x92;

    private const int GD_D_pad_Left = 0x93;

    /* Reserved */
    private const int GD_Sys_Dock = 0xA0;
    private const int GD_Sys_Undock = 0xA1;
    private const int GD_Sys_Setup = 0xA2;
    private const int GD_Sys_Break = 0xA3;
    private const int GD_Sys_Debugger_Break = 0xA4;
    private const int GD_Application_Break = 0xA5;
    private const int GD_Application_Debugger_Break = 0xA6;
    private const int GD_Sys_Speaker_Mute = 0xA7;

    private const int GD_Sys_Hibernate = 0xA8;

    /* Reserved */
    private const int GD_Sys_Display_Invert = 0xB0;
    private const int GD_Sys_Display_Internal = 0xB1;
    private const int GD_Sys_Display_External = 0xB2;
    private const int GD_Sys_Display_Both = 0xB3;
    private const int GD_Sys_Display_Dual = 0xB4;
    private const int GD_Sys_Display_Toggle = 0xB5;
    private const int GD_Sys_Display_Swap = 0xB6;

    private const int GD_Sys_Display_LCD_Autoscale = 0xB7;
    /* Reserved */


    /* Usage Page: Simulation Controls Page (= 0x02U)
    ** SimuDev: Simulation Device
    */
    private const int SC_SimuDev_Flight = 0x01;
    private const int SC_SimuDev_Automobile = 0x02;
    private const int SC_SimuDev_Tank = 0x03;
    private const int SC_SimuDev_Spaceship = 0x04;
    private const int SC_SimuDev_Submarine = 0x05;
    private const int SC_SimuDev_Sailing = 0x06;
    private const int SC_SimuDev_Motorcycle = 0x07;
    private const int SC_SimuDev_Sports = 0x08;
    private const int SC_SimuDev_Airplane = 0x09;
    private const int SC_SimuDev_Helicopter = 0x0A;
    private const int SC_SimuDev_MagicCarpet = 0x0B;

    private const int SC_SimuDev_Bicycle = 0x0C;

    /* Reserved */
    private const int SC_Flight_Control_Stick = 0x20;
    private const int SC_Flilght_Stick = 0x21;
    private const int SC_Cyclic_Control = 0x22;
    private const int SC_Cyclic_Trim = 0x23;
    private const int SC_Flight_Yoke = 0x24;

    private const int SC_Track_Control = 0x25;

    /* Reserved */
    private const int SC_Aileron = 0xB0;
    private const int SC_Aileron_Trim = 0xB1;
    private const int SC_Anti_Torque_Control = 0xB2;
    private const int SC_Autopilot_Enable = 0xB3;
    private const int SC_Chaff_Release = 0xB4;
    private const int SC_Collective_Control = 0xB5;
    private const int SC_Dive_Brake = 0xB6;
    private const int SC_Electronic_Countermeasures = 0xB7;
    private const int SC_Elevator = 0xB8;
    private const int SC_Elevator_Trim = 0xB9;
    private const int SC_Rudder = 0xBA;
    private const int SC_Throttle = 0xBB;
    private const int SC_Flight_Communications = 0xBC;
    private const int SC_Flare_Release = 0xBD;
    private const int SC_Landing_Gear = 0xBE;
    private const int SC_Toe_Brake = 0xBF;
    private const int SC_Trigger = 0xC0;
    private const int SC_Weapons_Arm = 0xC1;
    private const int SC_Weapons_Select = 0xC2;
    private const int SC_Wing_Flaps = 0xC3;
    private const int SC_Accelerator = 0xC4;
    private const int SC_Brake = 0xC5;
    private const int SC_Clutch = 0xC6;
    private const int SC_Shifter = 0xC7;
    private const int SC_Steering = 0xC8;
    private const int SC_Turret_Direction = 0xC9;
    private const int SC_Barrel_Elevation = 0xCA;
    private const int SC_Dive_Plane = 0xCB;
    private const int SC_Ballast = 0xCC;
    private const int SC_Bicycle_Crank = 0xCD;
    private const int SC_Handle_Bars = 0xCE;
    private const int SC_Front_Brake = 0xCF;

    private const int SC_Rear_Brake = 0xD0;
    /* Reserved */


    /* Usage Page: VR Controls Page (= 0x03)
    ** 
    */
    private const int VR_Belt = 0x01;
    private const int VR_Body_Suit = 0x02;
    private const int VR_Flexor = 0x03;
    private const int VR_Glove = 0x04;
    private const int VR_Head_Tracker = 0x05;
    private const int VR_Head_Mounted_Display = 0x06;
    private const int VR_Hand_Tracker = 0x07;
    private const int VR_Oculometer = 0x08;
    private const int VR_Vest = 0x09;

    private const int VR_Animatronic_Device = 0x0A;

    /* Reserved */
    private const int VR_Stereo_Enable = 0x20;

    private const int VR_Display_Enable = 0x21;
    /* Reserved */


    /* Usage Page: Sport Controls Page (= 0x04)
    ** 
    */
    private const int SpC_Baseball_Bat = 0x01;
    private const int SpC_Golf_Club = 0x02;
    private const int SpC_Rowing_Machine = 0x03;

    private const int SpC_Treadmill = 0x04;

    /* Reserved */
    private const int SpC_Oar = 0x30;
    private const int SpC_Slope = 0x31;
    private const int SpC_Rate = 0x32;
    private const int SpC_Stick_Speed = 0x33;
    private const int SpC_Stick_Face_Angle = 0x34;
    private const int SpC_Stick_HeelorToe = 0x35;
    private const int SpC_Stick_Follow_Through = 0x36;
    private const int SpC_Stick_Tempo = 0x37;
    private const int SpC_Stick_Type = 0x38;

    private const int SpC_Stick_Height = 0x39;

    /* Reserved */
    private const int SpC_Putter = 0x50;
    private const int SpC_Iron_1 = 0x51;
    private const int SpC_Iron_2 = 0x52;
    private const int SpC_Iron_3 = 0x53;
    private const int SpC_Iron_4 = 0x54;
    private const int SpC_Iron_5 = 0x55;
    private const int SpC_Iron_6 = 0x56;
    private const int SpC_Iron_7 = 0x57;
    private const int SpC_Iron_8 = 0x58;
    private const int SpC_Iron_9 = 0x59;
    private const int SpC_Iron_10 = 0x5A;
    private const int SpC_Iron_11 = 0x5B;
    private const int SpC_Sand_Wedge = 0x5C;
    private const int SpC_Loft_Wedge = 0x5D;
    private const int SpC_Power_Wedge = 0x5E;
    private const int SpC_Wood_1 = 0x5F;
    private const int SpC_Wood_3 = 0x60;
    private const int SpC_Wood_5 = 0x61;
    private const int SpC_Wood_7 = 0x62;

    private const int SpC_Wood_9 = 0x63;
    /* Reserved */


    /* Usage Page: Game Controls Page (= 0x05)
    */
    private const int GC_3D_Game_Controller = 0x01;
    private const int GC_Pinball_Device = 0x02;

    private const int GC_Gun_Device = 0x03;

    /* Reserved */
    private const int GC_Point_of_View = 0x20;
    private const int GC_Turn_Right_Left = 0x21;
    private const int GC_Pitch_Forward_Backward = 0x22;
    private const int GC_Roll_Right_Left = 0x23;
    private const int GC_Move_Right_Left = 0x24;
    private const int GC_Move_Forward_Backward = 0x25;
    private const int GC_Move_Up_Down = 0x26;
    private const int GC_Lean_Right_Left = 0x27;
    private const int GC_Lean_Forward_Backward = 0x28;
    private const int GC_Height_of_POV = 0x29;
    private const int GC_Flipper = 0x2A;
    private const int GC_Secondary_Flipper = 0x2B;
    private const int GC_Bump = 0x2C;
    private const int GC_New_Game = 0x2D;
    private const int GC_Shoot_Ball = 0x2E;
    private const int GC_Player = 0x2F;
    private const int GC_Gun_Bolt = 0x30;
    private const int GC_Gun_Clip = 0x31;
    private const int GC_Gun_Selector = 0x32;
    private const int GC_Gun_Single_Shot = 0x33;
    private const int GC_Gun_Burst = 0x34;
    private const int GC_Gun_Automatic = 0x35;
    private const int GC_Gun_Safety = 0x36;

    private const int GC_Gamepad_Fire_Jump = 0x37;

    /* Reserved */
    private const int GC_Gamepad_Trigger = 0x39;
    /* Reserved */


    /* Usage Page: Generic Device Controls (= 0x06)
    ** SC: Security Code
    */
    /* = 0x00 Undefined */
    /* Reserved */
    private const int GDC_Battery_Strength = 0x20;
    private const int GDC_Wireless_Channel = 0x21;
    private const int GDC_Wireless_ID = 0x22;
    private const int GDC_Discover_Wireless_Ctrl = 0x23;
    private const int GDC_SC_Character_Entered = 0x24;
    private const int GDC_SC_Character_Cleared = 0x25;

    private const int GDC_SC_Cleared = 0x26;
    /* Reserved */

    /* Usage Page: Keyboard/Keypad (= 0x07)
    */
    //TODO:

    /* Usage Page: LED (= 0x08)
    */
    private const int LED_Num_Lock = 0x01;
    private const int LED_Caps_Lock = 0x02;
    private const int LED_Scroll_Lock = 0x03;
    private const int LED_Compose = 0x04;
    private const int LED_Kana = 0x05;
    private const int LED_Power = 0x06;
    private const int LED_Shift = 0x07;
    private const int LED_Donot_Disturb = 0x08;
    private const int LED_Mute = 0x09;
    private const int LED_Tone_Enable = 0x0A;
    private const int LED_High_Cut_Filter = 0x0B;
    private const int LED_Low_Cut_Filter = 0x0C;
    private const int LED_Equalizer_Enable = 0x0D;
    private const int LED_Sound_Field_On = 0x0E;
    private const int LED_Surround_On = 0x0F;
    private const int LED_Repeat = 0x10;
    private const int LED_Stereo = 0x11;
    private const int LED_Sampling_Rate_Detect = 0x12;
    private const int LED_Spinning = 0x13;
    private const int LED_CAV = 0x14;
    private const int LED_CLV = 0x15;
    private const int LED_Recording_Format_Detect = 0x16;
    private const int LED_Off_Hook = 0x17;
    private const int LED_Ring = 0x18;
    private const int LED_Message_Waiting = 0x19;
    private const int LED_Data_Mode = 0x1A;
    private const int LED_Battery_Operation = 0x1B;
    private const int LED_Battery_OK = 0x1C;
    private const int LED_Battery_Low = 0x1D;
    private const int LED_Speaker = 0x1E;
    private const int LED_Head_Set = 0x1F;
    private const int LED_Hold = 0x20;
    private const int LED_Microphone = 0x21;
    private const int LED_Coverage = 0x22;
    private const int LED_Night_Mode = 0x23;
    private const int LED_Send_Calls = 0x24;
    private const int LED_Call_Pickup = 0x25;
    private const int LED_Conference = 0x26;
    private const int LED_Standby = 0x27;
    private const int LED_Camera_On = 0x28;
    private const int LED_Camera_Off = 0x29;
    private const int LED_On_Line = 0x2A;
    private const int LED_Off_Line = 0x2B;
    private const int LED_Busy = 0x2C;
    private const int LED_Ready = 0x2D;
    private const int LED_Paper_Out = 0x2E;
    private const int LED_Paper_Jam = 0x2F;
    private const int LED_Remote = 0x30;
    private const int LED_Forward = 0x31;
    private const int LED_Reverse = 0x32;
    private const int LED_Stop = 0x33;
    private const int LED_Rewind = 0x34;
    private const int LED_Fast_Forward = 0x35;
    private const int LED_Play = 0x36;
    private const int LED_Pause = 0x37;
    private const int LED_Record = 0x38;
    private const int LED_Error = 0x39;
    private const int LED_Selected_Indicator = 0x3A;
    private const int LED_In_Use_Indicator = 0x3B;
    private const int LED_Multi_Mode_Indicator = 0x3C;
    private const int LED_Indicator_On = 0x3D;
    private const int LED_Indicator_Flash = 0x3E;
    private const int LED_Indicator_Slow_Blink = 0x3F;
    private const int LED_Indicator_Fast_Blink = 0x40;
    private const int LED_Indicator_Off = 0x41;
    private const int LED_Flash_On_Time = 0x42;
    private const int LED_Slow_Blink_On_Time = 0x43;
    private const int LED_Slow_Blink_Off_Time = 0x44;
    private const int LED_Fast_Blink_On_Time = 0x45;
    private const int LED_Fast_Blink_Off_Time = 0x46;
    private const int LED_Usage_Indicator_Color = 0x47;
    private const int LED_Indicator_Red = 0x48;
    private const int LED_Indicator_Green = 0x49;
    private const int LED_Indicator_Amber = 0x4A;
    private const int LED_Generic_Indicator = 0x4B;
    private const int LED_Sys_Suspend = 0x4C;

    private const int LED_External_Power_Connected = 0x4D;
    /* Reserved */


    /* Usage Page: Button (= 0x09)
    ** ID N is Button N 
    */

    /* Usage Page: Ordinal (= 0x0A)
    ** ID N is Instance N
    */

    /* Usage Page: Telephony Device (= 0x0B)
    */
    private const int TD_Phone = 0x01;
    private const int TD_Answering_Machine = 0x02;
    private const int TD_Message_Controls = 0x03;
    private const int TD_Handset = 0x04;
    private const int TD_Headset = 0x05;
    private const int TD_Telephony_Key_Pad = 0x06;

    private const int TD_Programmable_Button = 0x07;

    /* Reserved */
    private const int TD_Hook_Switch = 0x20;
    private const int TD_Flash = 0x21;
    private const int TD_Feature = 0x22;
    private const int TD_Hold = 0x23;
    private const int TD_Redial = 0x24;
    private const int TD_Transfer = 0x25;
    private const int TD_Drop = 0x26;
    private const int TD_Park = 0x27;
    private const int TD_Forward_Calls = 0x28;
    private const int TD_Alternate_Function = 0x29;
    private const int TD_Line = 0x2A;
    private const int TD_Speaker_Phone = 0x2B;
    private const int TD_Conference = 0x2C;
    private const int TD_Ring_Enable = 0x2D;
    private const int TD_Ring_Select = 0x2E;
    private const int TD_Phone_Mute = 0x2F;
    private const int TD_Caller_ID = 0x30;

    private const int TD_Send = 0x31;

    /* Reserved */
    private const int TD_Speed_Dial = 0x50;
    private const int TD_Store_Number = 0x51;
    private const int TD_Recall_Number = 0x52;

    private const int TD_Phone_Directory = 0x53;

    /* Reserved */
    private const int TD_Voice_Mail = 0x70;
    private const int TD_Screen_Calls = 0x71;
    private const int TD_Do_Not_Disturb = 0x72;
    private const int TD_Message = 0x73;

    private const int TD_Answer_On_Off = 0x74;

    /* Reserved */
    private const int TD_Inside_Dial_Tone = 0x90;
    private const int TD_Outside_Dial_Tone = 0x91;
    private const int TD_Inside_Ring_Tone = 0x92;
    private const int TD_Outside_Ring_Tone = 0x93;
    private const int TD_Priority_Ring_Tone = 0x94;
    private const int TD_Inside_Ringback = 0x95;
    private const int TD_Priority_Ringback = 0x96;
    private const int TD_Line_Busy_Tone = 0x97;
    private const int TD_Reorder_Tone = 0x98;
    private const int TD_Call_Waiting_Tone = 0x99;
    private const int TD_Confirmation_Tone_1 = 0x9A;
    private const int TD_Confirmation_Tone_2 = 0x9B;
    private const int TD_Tones_Off = 0x9C;
    private const int TD_Outside_Ringback = 0x9D;

    private const int TD_Ringer = 0x9E;

    /* Reserved */
    private const int TD_Phone_Key_0 = 0xB0;
    private const int TD_Phone_Key_1 = 0xB1;
    private const int TD_Phone_Key_2 = 0xB2;
    private const int TD_Phone_Key_3 = 0xB3;
    private const int TD_Phone_Key_4 = 0xB4;
    private const int TD_Phone_Key_5 = 0xB5;
    private const int TD_Phone_Key_6 = 0xB6;
    private const int TD_Phone_Key_7 = 0xB7;
    private const int TD_Phone_Key_8 = 0xB8;
    private const int TD_Phone_Key_9 = 0xB9;
    private const int TD_Phone_Key_Star = 0xBA;
    private const int TD_Phone_Key_Pound = 0xBB;
    private const int TD_Phone_Key_A = 0xBC;
    private const int TD_Phone_Key_B = 0xBD;
    private const int TD_Phone_Key_C = 0xBE;

    private const int TD_Phone_Key_D = 0xBF;
    /* Reserved */

    /* Usage Page: Consumer (= 0x0C)
    ** App      - Application
    ** Btn      - Button
    ** Ctrl     - Control
    ** Incr/Decr - Increase/Decrease
    ** Prog     - Programmable
    ** Sel      - Select
    ** Sys      - System
    ** ILL      - Illumination
    */

    private const int UC_Consumer_Ctrl = 0x1;
    private const int UC_Numeric_Keypad = 0x2;
    private const int UC_Prog_Btns = 0x3;
    private const int UC_Mic = 0x4;
    private const int UC_Headphone = 0x5;

    private const int UC_Graphic_Equalizer = 0x6;

    /* Reserved */
    private const int UC_Add_10 = 0x20;
    private const int UC_Add_100 = 0x21;

    private const int UC_AM_PM = 0x22;

    /* Reserved */
    private const int UC_Power = 0x30;
    private const int UC_Reset = 0x31;
    private const int UC_Sleep = 0x32;
    private const int UC_Sleep_After = 0x33;
    private const int UC_Sleep_Mode = 0x34;
    private const int UC_ILL = 0x35;

    private const int UC_Function_Buttons = 0x36;

    /* Reserved */
    private const int UC_Menu = 0x40;
    private const int UC_Menu_Pick = 0x41;
    private const int UC_Menu_Up = 0x42;
    private const int UC_Menu_Down = 0x43;
    private const int UC_Menu_Left = 0x44;
    private const int UC_Menu_Right = 0x45;
    private const int UC_Menu_Escape = 0x46;
    private const int UC_Menu_Value_Incr = 0x47;

    private const int UC_Menu_Value_Decr = 0x48;

    /* Reserved */
    private const int UC_Data_On_Screen = 0x60;
    private const int UC_Closed_Caption = 0x61;
    private const int UC_Closed_Caption_Sel = 0x62;
    private const int UC_VCR_TV = 0x63;
    private const int UC_Broadcast_Mode = 0x64;
    private const int UC_Snapshot = 0x65;

    private const int UC_Still = 0x66;

    /* Reserved */
    private const int UC_Selion = 0x80;
    private const int UC_Assign_Selion = 0x81;
    private const int UC_Mode_Step = 0x82;
    private const int UC_Recall_Last = 0x83;
    private const int UC_Enter_Channel = 0x84;
    private const int UC_Order_Movie = 0x85;
    private const int UC_Channel = 0x86;
    private const int UC_Media_Selion = 0x87;
    private const int UC_Media_Sel_Computer = 0x88;
    private const int UC_Media_Sel_TV = 0x89;
    private const int UC_Media_Sel_WWW = 0x8A;
    private const int UC_Media_Sel_DVD = 0x8B;
    private const int UC_Media_Sel_Telephone = 0x8C;
    private const int UC_Media_Sel_Program_Guide = 0x8D;
    private const int UC_Media_Sel_Video_Phone = 0x8E;
    private const int UC_Media_Sel_Games = 0x8F;
    private const int UC_Media_Sel_Messages = 0x90;
    private const int UC_Media_Sel_CD = 0x91;
    private const int UC_Media_Sel_VCR = 0x92;
    private const int UC_Media_Sel_Tuner = 0x93;
    private const int UC_Quit = 0x94;
    private const int UC_Help = 0x95;
    private const int UC_Media_Sel_Tape = 0x96;
    private const int UC_Media_Sel_Cable = 0x97;
    private const int UC_Media_Sel_Satellite = 0x98;
    private const int UC_Media_Sel_Security = 0x99;
    private const int UC_Media_Sel_Home = 0x9A;
    private const int UC_Media_Sel_Call = 0x9B;
    private const int UC_Channel_Incr = 0x9C;
    private const int UC_Channel_Decr = 0x9D;

    private const int UC_Media_Sel_SAP = 0x9E;

    /* Reserved */
    private const int UC_VCR_Plus = 0xA0;
    private const int UC_Once = 0xA1;
    private const int UC_Daily = 0xA2;
    private const int UC_Weekly = 0xA3;

    private const int UC_Monthly = 0xA4;

    /* Reserved */
    private const int UC_Play = 0xB0;
    private const int UC_Pause = 0xB1;
    private const int UC_Record = 0xB2;
    private const int UC_Fast_Forward = 0xB3;
    private const int UC_Rewind = 0xB4;
    private const int UC_Scan_Next_Track = 0xB5;
    private const int UC_Scan_Previous_Track = 0xB6;
    private const int UC_Stop = 0xB7;
    private const int UC_Eject = 0xB8;
    private const int UC_Random_Play = 0xB9;
    private const int UC_Sel_Disc = 0xBA;
    private const int UC_Enter_Disc = 0xBB;
    private const int UC_Repeat = 0xBC;
    private const int UC_Tracking = 0xBD;
    private const int UC_Track_Normal = 0xBE;
    private const int UC_Slow_Tracking = 0xBF;
    private const int UC_Frame_Forward = 0xC0;
    private const int UC_Frame_Back = 0xC1;
    private const int UC_Mark = 0xC2;
    private const int UC_Clear_Mark = 0xC3;
    private const int UC_Repeat_From_Mark = 0xC4;
    private const int UC_Return_To_Mark = 0xC5;
    private const int UC_Search_Mark_Forward = 0xC6;
    private const int UC_Search_Mark_Backward = 0xC7;
    private const int UC_Counter_Reset = 0xC8;
    private const int UC_Show_Counter = 0xC9;
    private const int UC_Tracking_Incr = 0xCA;
    private const int UC_Tracking_Decr = 0xCB;
    private const int UC_Stop_Eject = 0xCC;
    private const int UC_Play_Pause = 0xCD;

    private const int UC_Play_Skip = 0xCE;

    /* Reserved */
    private const int UC_Volume = 0xE0;
    private const int UC_Balance = 0xE1;
    private const int UC_Mute = 0xE2;
    private const int UC_Bass = 0xE3;
    private const int UC_Treble = 0xE4;
    private const int UC_Bass_Boost = 0xE5;
    private const int UC_Surround_Mode = 0xE6;
    private const int UC_Loudness = 0xE7;
    private const int UC_MPX = 0xE8;
    private const int UC_Volume_Incr = 0xE9;

    private const int UC_Volume_Decr = 0xEA;

    /* Reserved */
    private const int UC_Speed_Sel = 0xF0;
    private const int UC_Playback_Speed = 0xF1;
    private const int UC_Standard_Play = 0xF2;
    private const int UC_Long_Play = 0xF3;
    private const int UC_Extended_Play = 0xF4;

    private const int UC_Slow = 0xF5;

    /* Reserved */
    private const int UC_Fan_Enable = 0x100;
    private const int UC_Fan_Speed = 0x101;
    private const int UC_Light_Enable = 0x102;
    private const int UC_Light_ILL_Level = 0x103;
    private const int UC_Climate_Ctrl_Enable = 0x104;
    private const int UC_Room_Temperature = 0x105;
    private const int UC_Security_Enable = 0x106;
    private const int UC_Fire_Alarm = 0x107;
    private const int UC_Police_Alarm = 0x108;
    private const int UC_Proximity = 0x109;
    private const int UC_Motion = 0x10A;
    private const int UC_Duress_Alarm = 0x10B;
    private const int UC_Holdup_Alarm = 0x10C;

    private const int UC_Medical_Alarm = 0x10D;

    /* Reserved */
    private const int UC_Balance_Right = 0x150;
    private const int UC_Balance_Left = 0x151;
    private const int UC_Bass_Incr = 0x152;
    private const int UC_Bass_Decr = 0x153;
    private const int UC_Treble_Incr = 0x154;

    private const int UC_Treble_Decr = 0x155;

    /* Reserved */
    private const int UC_Speaker_Sys = 0x160;
    private const int UC_Channel_Left = 0x161;
    private const int UC_Channel_Right = 0x162;
    private const int UC_Channel_Center = 0x163;
    private const int UC_Channel_Front = 0x164;
    private const int UC_Channel_Center_Front = 0x165;
    private const int UC_Channel_Side = 0x166;
    private const int UC_Channel_Surround = 0x167;
    private const int UC_Channel_Low_Frequency_Enhancement = 0x168;
    private const int UC_Channel_Top = 0x169;

    private const int UC_Channel_Unknown = 0x16A;

    /* Reserved */
    private const int UC_Subchannel = 0x170;
    private const int UC_Subchannel_Incr = 0x171;
    private const int UC_Subchannel_Decr = 0x172;
    private const int UC_Alternate_Audio_Incr = 0x173;

    private const int UC_Alternate_Audio_Decr = 0x174;

    /* Reserved */
    private const int UC_App_Launch_Btns = 0x180;
    private const int UC_AL_Launch_Btn_Config_Tool = 0x181;
    private const int UC_AL_Prog_Btn_Config = 0x182;
    private const int UC_AL_Consumer_Ctrl_Config = 0x183;
    private const int UC_AL_Word_Processor = 0x184;
    private const int UC_AL_Text_Editor = 0x185;
    private const int UC_AL_Spreadsheet = 0x186;
    private const int UC_AL_Graphics_Editor = 0x187;
    private const int UC_AL_Presentation_App = 0x188;
    private const int UC_AL_Database_App = 0x189;
    private const int UC_AL_Email_Reader = 0x18A;
    private const int UC_AL_Newsreader = 0x18B;
    private const int UC_AL_Voicemail = 0x18C;
    private const int UC_AL_Contacts_Address_Book = 0x18D;
    private const int UC_AL_Calendar_Schedule = 0x18E;
    private const int UC_AL_Task_Project_Manager = 0x18F;
    private const int UC_AL_Log_Journal_Timecard = 0x190;
    private const int UC_AL_Checkbook_Finance = 0x191;
    private const int UC_AL_Calculator = 0x192;
    private const int UC_AL_AV_Capture_Playback = 0x193;
    private const int UC_AL_Local_Machine_Browser = 0x194;
    private const int UC_AL_LAN_WAN_Browser = 0x195;
    private const int UC_AL_Internet_Browser = 0x196;
    private const int UC_AL_RemoteNet_ISP_Connect = 0x197;
    private const int UC_AL_Net_Conference = 0x198;
    private const int UC_AL_Net_Chat = 0x199;
    private const int UC_AL_Telephony_Dialer = 0x19A;
    private const int UC_AL_Logon = 0x19B;
    private const int UC_AL_Logoff = 0x19C;
    private const int UC_AL_Logon_Logoff = 0x19D;
    private const int UC_AL_Terminal_Lock_Screensaver = 0x19E;
    private const int UC_AL_Ctrl_Panel = 0x19F;
    private const int UC_AL_Command_Line_Processor_Run = 0x1A0;
    private const int UC_AL_Process_Task_Manager = 0x1A1;
    private const int UC_AL_Sel_Task_App = 0x1A2;
    private const int UC_AL_Next_Task_App = 0x1A3;
    private const int UC_AL_Previous_Task_App = 0x1A4;
    private const int UC_AL_Preemptive_Halt_Task_App = 0x1A5;
    private const int UC_AL_Integrated_Help_Center = 0x1A6;
    private const int UC_AL_Documents = 0x1A7;
    private const int UC_AL_Thesaurus = 0x1A8;
    private const int UC_AL_Dictionary = 0x1A9;
    private const int UC_AL_Desktop = 0x1AA;
    private const int UC_AL_Spell_Check = 0x1AB;
    private const int UC_AL_Grammar_Check = 0x1AC;
    private const int UC_AL_Wireless_Status = 0x1AD;
    private const int UC_AL_Keyboard_Layout = 0x1AE;
    private const int UC_AL_Virus_Protection = 0x1AF;
    private const int UC_AL_Encryption = 0x1B0;
    private const int UC_AL_Screen_Saver = 0x1B1;
    private const int UC_AL_Alarms = 0x1B2;
    private const int UC_AL_Clock = 0x1B3;
    private const int UC_AL_File_Browser = 0x1B4;
    private const int UC_AL_Power_Status = 0x1B5;
    private const int UC_AL_Image_Browser = 0x1B6;
    private const int UC_AL_Audio_Browser = 0x1B7;
    private const int UC_AL_Movie_Browser = 0x1B8;
    private const int UC_AL_Digital_Rights_Manager = 0x1B9;
    private const int UC_AL_Digital_Wallet = 0x1BA;
    private const int UC_AL_Instant_Messaging = 0x1BC;
    private const int UC_AL_OEM_Features_Tips_Tutorial_Browser = 0x1BD;
    private const int UC_AL_OEM_Help = 0x1BE;
    private const int UC_AL_Online_Community = 0x1BF;
    private const int UC_AL_Entertainment_Content_Browser = 0x1C0;
    private const int UC_AL_Online_Shopping_Browser = 0x1C1;
    private const int UC_AL_SmartCard_Information_Help = 0x1C2;
    private const int UC_AL_Market_Monitoror_Finance_Browser = 0x1C3;
    private const int UC_AL_Customized_Corporate_News_Browser = 0x1C4;
    private const int UC_AL_Online_Activity_Browser = 0x1C5;
    private const int UC_AL_Research_Search_Browser = 0x1C6;

    private const int UC_AL_Audio_Player = 0x1C7;

    /* Reserved */
    private const int UC_Generic_GUI_App_Ctrls = 0x200;
    private const int UC_AC_New = 0x201;
    private const int UC_AC_Open = 0x202;
    private const int UC_AC_Close = 0x203;
    private const int UC_AC_Exit = 0x204;
    private const int UC_AC_Maximize = 0x205;
    private const int UC_AC_Minimize = 0x206;
    private const int UC_AC_Save = 0x207;
    private const int UC_AC_Print = 0x208;

    private const int UC_AC_Properties = 0x209;

    /* Reserved */
    private const int UC_AC_Undo = 0x21A;
    private const int UC_AC_Copy = 0x21B;
    private const int UC_AC_Cut = 0x21C;
    private const int UC_AC_Paste = 0x21D;
    private const int UC_AC_Sel_All = 0x21E;

    private const int UC_AC_Find = 0x21F;

    /* Reserved */
    private const int UC_AC_Find_and_Replace = 0x220;
    private const int UC_AC_Search = 0x221;
    private const int UC_AC_Go_To = 0x222;
    private const int UC_AC_Home = 0x223;
    private const int UC_AC_Back = 0x224;
    private const int UC_AC_Forward = 0x225;
    private const int UC_AC_Stop = 0x226;
    private const int UC_AC_Refresh = 0x227;
    private const int UC_AC_Previous_Link = 0x228;
    private const int UC_AC_Next_Link = 0x229;
    private const int UC_AC_Bookmarks = 0x22A;
    private const int UC_AC_History = 0x22B;
    private const int UC_AC_Subscriptions = 0x22C;
    private const int UC_AC_Zoom_In = 0x22D;
    private const int UC_AC_Zoom_Out = 0x22E;
    private const int UC_AC_Zoom = 0x22F;
    private const int UC_AC_Full_Screen_View = 0x230;
    private const int UC_AC_Normal_View = 0x231;
    private const int UC_AC_View_Toggle = 0x232;
    private const int UC_AC_Scroll_Up = 0x233;
    private const int UC_AC_Scroll_Down = 0x234;
    private const int UC_AC_Scroll = 0x235;
    private const int UC_AC_Pan_Left = 0x236;
    private const int UC_AC_Pan_Right = 0x237;
    private const int UC_AC_Pan = 0x238;
    private const int UC_AC_New_Window = 0x239;
    private const int UC_AC_Tile_Horizontally = 0x23A;
    private const int UC_AC_Tile_Vertically = 0x23B;
    private const int UC_AC_Format = 0x23C;
    private const int UC_AC_Edit = 0x23D;
    private const int UC_AC_Bold = 0x23E;
    private const int UC_AC_Italics = 0x23F;
    private const int UC_AC_Underline = 0x240;
    private const int UC_AC_Strikethrough = 0x241;
    private const int UC_AC_Subscript = 0x242;
    private const int UC_AC_Superscript = 0x243;
    private const int UC_AC_All_Caps = 0x244;
    private const int UC_AC_Rotate = 0x245;
    private const int UC_AC_Resize = 0x246;
    private const int UC_AC_Flip_Horiz = 0x247;
    private const int UC_AC_Flip_Verti = 0x248;
    private const int UC_AC_Mirror_Horizontal = 0x249;
    private const int UC_AC_Mirror_Vertical = 0x24A;
    private const int UC_AC_Font_Sel = 0x24B;
    private const int UC_AC_Font_Color = 0x24C;
    private const int UC_AC_Font_Size = 0x24D;
    private const int UC_AC_Justify_Left = 0x24E;
    private const int UC_AC_Justify_Center_H = 0x24F;
    private const int UC_AC_Justify_Right = 0x250;
    private const int UC_AC_Justify_Block_H = 0x251;
    private const int UC_AC_Justify_Top = 0x252;
    private const int UC_AC_Justify_Center_V = 0x253;
    private const int UC_AC_Justify_Bottom = 0x254;
    private const int UC_AC_Justify_Block_V = 0x255;
    private const int UC_AC_Indent_Decr = 0x256;
    private const int UC_AC_Indent_Incr = 0x257;
    private const int UC_AC_Numbered_List = 0x258;
    private const int UC_AC_Restart_Numbering = 0x259;
    private const int UC_AC_Bulleted_List = 0x25A;
    private const int UC_AC_Promote = 0x25B;
    private const int UC_AC_Demote = 0x25C;
    private const int UC_AC_Yes = 0x25D;
    private const int UC_AC_No = 0x25E;
    private const int UC_AC_Cancel = 0x25F;
    private const int UC_AC_Catalog = 0x260;
    private const int UC_AC_BuyorCheckout = 0x261;
    private const int UC_AC_Add_to_Cart = 0x262;
    private const int UC_AC_Expand = 0x263;
    private const int UC_AC_Expand_All = 0x264;
    private const int UC_AC_Collapse = 0x265;
    private const int UC_AC_Collapse_All = 0x266;
    private const int UC_AC_Print_Preview = 0x267;
    private const int UC_AC_Paste_Special = 0x268;
    private const int UC_AC_Insert_Mode = 0x269;
    private const int UC_AC_Delete = 0x26A;
    private const int UC_AC_Lock = 0x26B;
    private const int UC_AC_Unlock = 0x26C;
    private const int UC_AC_Protect = 0x26D;
    private const int UC_AC_Unprotect = 0x26E;
    private const int UC_AC_Attach_Comment = 0x26F;
    private const int UC_AC_Delete_Comment = 0x270;
    private const int UC_AC_View_Comment = 0x271;
    private const int UC_AC_Sel_Word = 0x272;
    private const int UC_AC_Sel_Sentence = 0x273;
    private const int UC_AC_Sel_Paragraph = 0x274;
    private const int UC_AC_Sel_Column = 0x275;
    private const int UC_AC_Sel_Row = 0x276;
    private const int UC_AC_Sel_Table = 0x277;
    private const int UC_AC_Sel_Object = 0x278;
    private const int UC_AC_Redo_Repeat = 0x279;
    private const int UC_AC_Sort = 0x27A;
    private const int UC_AC_Sort_Ascending = 0x27B;
    private const int UC_AC_Sort_Descending = 0x27C;
    private const int UC_AC_Filter = 0x27D;
    private const int UC_AC_Set_Clock = 0x27E;
    private const int UC_AC_View_Clock = 0x27F;
    private const int UC_AC_Sel_Time_Zone = 0x280;
    private const int UC_AC_Edit_Time_Zones = 0x281;
    private const int UC_AC_Set_Alarm = 0x282;
    private const int UC_AC_Clear_Alarm = 0x283;
    private const int UC_AC_Snooze_Alarm = 0x284;
    private const int UC_AC_Reset_Alarm = 0x285;
    private const int UC_AC_Synchronize = 0x286;
    private const int UC_AC_Send_or_Recv = 0x287;
    private const int UC_AC_Send_To = 0x288;
    private const int UC_AC_Reply = 0x289;
    private const int UC_AC_Reply_All = 0x28A;
    private const int UC_AC_Forward_Msg = 0x28B;
    private const int UC_AC_Send = 0x28C;
    private const int UC_AC_Attach_File = 0x28D;
    private const int UC_AC_Upload = 0x28E;
    private const int UC_AC_Download_Save_As = 0x28F;
    private const int UC_AC_Set_Borders = 0x290;
    private const int UC_AC_Insert_Row = 0x291;
    private const int UC_AC_Insert_Column = 0x292;
    private const int UC_AC_Insert_File = 0x293;
    private const int UC_AC_Insert_Picture = 0x294;
    private const int UC_AC_Insert_Object = 0x295;
    private const int UC_AC_Insert_Symbol = 0x296;
    private const int UC_AC_Save_and_Close = 0x297;
    private const int UC_AC_Rename = 0x298;
    private const int UC_AC_Merge = 0x299;
    private const int UC_AC_Split = 0x29A;
    private const int UC_AC_Distribute_Horiz = 0x29B;

    private const int UC_AC_Distribute_Verti = 0x29C;
    /* Reserved */


    /* Usage Page: Digitizer (= 0x0D)
    */
    private const int D_Digitizer = 0x1;
    private const int D_Pen = 0x2;
    private const int D_Light_Pen = 0x3;
    private const int D_Touch_Screen = 0x4;
    private const int D_Touch_Pad = 0x5;
    private const int D_White_Board = 0x6;
    private const int D_Coordinate_Measuring_Machine = 0x7;
    private const int D_4D_Digitizer = 0x8;
    private const int D_Stereo_Plotter = 0x9;
    private const int D_Articulated_Arm = 0xA;
    private const int D_Armature = 0xB;
    private const int D_Multiple_Point_Digitizer = 0xC;

    private const int D_Free_Space_Wand = 0xD;

    /* Reserved */
    private const int D_Stylus = 0x20;
    private const int D_Puck = 0x21;

    private const int D_Finger = 0x22;

    /* Reserved */
    private const int D_Tip_Pressure = 0x30;
    private const int D_Barrel_Pressure = 0x31;
    private const int D_In_Range = 0x32;
    private const int D_Touch = 0x33;
    private const int D_Untouch = 0x34;
    private const int D_Tap = 0x35;
    private const int D_Quality = 0x36;
    private const int D_Data_Valid = 0x37;
    private const int D_Transducer_Index = 0x38;
    private const int D_Tablet_Function_Keys = 0x39;
    private const int D_Program_Change_Keys = 0x3A;
    private const int D_Battery_Strength = 0x3B;
    private const int D_Invert = 0x3C;
    private const int D_X_Tilt = 0x3D;
    private const int D_Y_Tilt = 0x3E;
    private const int D_Azimuth = 0x3F;
    private const int D_Altitude = 0x40;
    private const int D_Twist = 0x41;
    private const int D_Tip_Switch = 0x42;
    private const int D_Secondary_Tip_Switch = 0x43;
    private const int D_Barrel_Switch = 0x44;
    private const int D_Eraser = 0x45;

    private const int D_Tablet_Pick = 0x46;
    /* Reserved */


    /* Usage Page: Alphanumeric Display (= 0x14)
    */
    private const int AD_Alphanumeric_Display = 0x1;

    private const int AD_Bitmapped_Display = 0x2;

    /* Reserved */
    private const int AD_Display_Attributes_Report = 0x20;
    private const int AD_ASCII_Character_Set = 0x21;
    private const int AD_Data_Read_Back = 0x22;
    private const int AD_Font_Read_Back = 0x23;
    private const int AD_Display_Control_Report = 0x24;
    private const int AD_Clear_Display = 0x25;
    private const int AD_Display_Enable = 0x26;
    private const int AD_Screen_Saver_Delay = 0x27;
    private const int AD_Screen_Saver_Enable = 0x28;
    private const int AD_Vertical_Scroll = 0x29;
    private const int AD_Horizontal_Scroll = 0x2A;
    private const int AD_Character_Report = 0x2B;
    private const int AD_Display_Data = 0x2C;
    private const int AD_Display_Status = 0x2D;
    private const int AD_Stat_Not_Ready = 0x2E;
    private const int AD_Stat_Ready = 0x2F;
    private const int AD_Err_Not_a_loadable_character = 0x30;
    private const int AD_Err_Font_data_cannot_be_read = 0x31;
    private const int AD_Cursor_Position_Report = 0x32;
    private const int AD_Row = 0x33;
    private const int AD_Column = 0x34;
    private const int AD_Rows = 0x35;
    private const int AD_Columns = 0x36;
    private const int AD_Cursor_Pixel_Positioning = 0x37;
    private const int AD_Cursor_Mode = 0x38;
    private const int AD_Cursor_Enable = 0x39;
    private const int AD_Cursor_Blink = 0x3A;
    private const int AD_Font_Report = 0x3B;
    private const int AD_Font_Data = 0x3C;
    private const int AD_Character_Width = 0x3D;
    private const int AD_Character_Height = 0x3E;
    private const int AD_Character_Spacing_Horizontal = 0x3F;
    private const int AD_Character_Spacing_Vertical = 0x40;
    private const int AD_Unicode_Character_Set = 0x41;
    private const int AD_Font_7_Segment = 0x42;
    private const int AD_7_Segment_Direct_Map = 0x43;
    private const int AD_Font_14_Segment = 0x44;
    private const int AD_14_Segment_Direct_Map = 0x45;
    private const int AD_Display_Brightness = 0x46;
    private const int AD_Display_Contrast = 0x47;
    private const int AD_Character_Attribute = 0x48;
    private const int AD_Attribute_Readback = 0x49;
    private const int AD_Attribute_Data = 0x4A;
    private const int AD_Char_Attr_Enhance = 0x4B;
    private const int AD_Char_Attr_Underline = 0x4C;

    private const int AD_Char_Attr_Blink = 0x4D;

    /* Reserved */
    private const int AD_Bitmap_Size_X = 0x80;
    private const int AD_Bitmap_Size_Y = 0x81;
    private const int AD_Bit_Depth_Format = 0x83;
    private const int AD_Display_Orientation = 0x84;
    private const int AD_Palette_Report = 0x85;
    private const int AD_Palette_Data_Size = 0x86;
    private const int AD_Palette_Data_Offset = 0x87;
    private const int AD_Palette_Data = 0x88;
    private const int AD_Blit_Report = 0x8A;
    private const int AD_Blit_Rectangle_X1 = 0x8B;
    private const int AD_Blit_Rectangle_Y1 = 0x8C;
    private const int AD_Blit_Rectangle_X2 = 0x8D;
    private const int AD_Blit_Rectangle_Y2 = 0x8E;
    private const int AD_Blit_Data = 0x8F;
    private const int AD_Soft_Button = 0x90;
    private const int AD_Soft_Button_ID = 0x91;
    private const int AD_Soft_Button_Side = 0x92;
    private const int AD_Soft_Button_Offset_1 = 0x93;
    private const int AD_Soft_Button_Offset_2 = 0x94;

    private const int AD_Soft_Button_Report = 0x95;
    /* Reserved */


    /* Usage Page: Medical Instrument (= 0x40)
    */
    private const int MI_Medical_Ultrasound = 0x1;

    /* Reserved */
    private const int MI_VCR_Acquisition = 0x20;
    private const int MI_Freeze_Thaw = 0x21;
    private const int MI_Clip_Store = 0x22;
    private const int MI_Update = 0x23;
    private const int MI_Next = 0x24;
    private const int MI_Save = 0x25;
    private const int MI_Print = 0x26;

    private const int MI_Microphone_Enable = 0x27;

    /* Reserved */
    private const int MI_Cine = 0x40;
    private const int MI_Transmit_Power = 0x41;
    private const int MI_Volume = 0x42;
    private const int MI_Focus = 0x43;

    private const int MI_Depth = 0x44;

    /* Reserved */
    private const int MI_Soft_Step_Primary = 0x60;

    private const int MI_Soft_Step_Secondary = 0x61;

    /* Reserved */
    private const int MI_Depth_Gain_Compensation = 0x70;

    /* Reserved */
    private const int MI_Zoom_Select = 0x80;
    private const int MI_Zoom_Adjust = 0x81;
    private const int MI_Spectral_Doppler_Mode_Select = 0x82;
    private const int MI_Spectral_Doppler_Adjust = 0x83;
    private const int MI_Color_Doppler_Mode_Select = 0x84;
    private const int MI_Color_Doppler_Adjust = 0x85;
    private const int MI_Motion_Mode_Select = 0x86;
    private const int MI_Motion_Mode_Adjust = 0x87;
    private const int MI_2D_Mode_Select = 0x88;

    private const int MI_2D_Mode_Adjust = 0x89;

    /* Reserved */
    private const int MI_Soft_Control_Select = 0xA0;

    private const int MI_Soft_Control_Adjust = 0xA1;
    /* Reserved */

    #endregion

    private static int ri_ItemSize(int sizeMask)
    {
        return ((byte)(sizeMask) == Size_4B ? 4 : (byte)(sizeMask));
    }

    private static unsafe int ri_GetItemData(byte* itemData, byte size)
    {
        return size switch
        {
            1 => *itemData,
            2 => *(short*)itemData,
            4 => *(int*)itemData,
            _ => 0
        };
    }

    public static unsafe bool Parse(byte[] descriptor)
    {
        byte space = 0;
        ushort index = 0;
        int sUsagePage = -1;

        while (index < descriptor.Length)
        {
            byte itemTag = (byte)(descriptor[index] & TAG_MASK);
            byte itemSize = (byte)ri_ItemSize(descriptor[index] & SIZE_MASK);
            int itemData = 0;

            if (index + itemSize >= descriptor.Length)
            {
                break;
            }

            fixed (byte* ptr = descriptor)
            {
                itemData = ri_GetItemData(&ptr[index + 1], itemSize);

                switch(itemTag & TYPE_MASK)
                {
                    case MAIN_ITEM:
                        //ri_MainItem(itemTag, itemData, &space);
                        break;
                    case GLOBAL_ITEM:
                        //ri_GlobalItem(itemTag, itemData, space, &sUsagePage);
                        break;
                    case LOCAL_ITEM:
                        //ri_LocalItem(itemTag, itemData, space, sUsagePage);
                        break;
                    default:
                        //LOG("Unknown Type: %02X, index: %d\r\n", itemTag, index);
                        break;
                }
            }

            index += (ushort)(itemSize + 1);
        }

        return index < descriptor.Length;
    }
}
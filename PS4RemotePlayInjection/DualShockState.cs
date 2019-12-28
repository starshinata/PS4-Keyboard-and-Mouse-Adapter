// PS4RemotePlayInterceptor (File: Classes/DualShockState.cs)
//
// Copyright (c) 2018 Komefai
//
// Visit http://komefai.com for more information
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace PS4RemotePlayInterceptor
{
    // References
    // https://github.com/Jays2Kings/DS4Windows/blob/jay/DS4Windows/DS4Library/DS4Device.cs
    // https://github.com/Jays2Kings/DS4Windows/blob/jay/DS4Windows/DS4Library/DS4Sixaxis.cs
    // https://github.com/Jays2Kings/DS4Windows/blob/jay/DS4Windows/DS4Library/DS4Touchpad.cs
    // http://www.psdevwiki.com/ps4/DS4-USB

    public class Touch
    {
        public byte TouchID { get; set; }
        public bool IsTouched { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        /* Constructors */
        public Touch()
        {
            TouchID = 0;
            IsTouched = false;
            X = 0;
            Y = 0;
        }
        public Touch(byte touchID, bool isTouched, int x, int y)
        {
            TouchID = touchID;
            IsTouched = isTouched;
            X = x;
            Y = y;
        }
        public Touch(Touch touch)
        {
            TouchID = touch.TouchID;
            IsTouched = touch.IsTouched;
            X = touch.X;
            Y = touch.Y;
        }

        public Touch Clone()
        {
            return new Touch(this);
        }
    }

    public class DualShockState
    {
        public const int TOUCHPAD_DATA_OFFSET = 35;

        private static DualShockState _defaultState = new DualShockState();
        private const byte AnalogDeadZoneMin = 0x78;
        private const byte AnalogDeadZoneCenter = 0x80;
        private const byte AnalogDeadZoneMax = 0x88;

        private enum VK : byte
        {
            L2 = 1 << 2,
            R2 = 1 << 3,
            Triangle = 1 << 7,
            Circle = 1 << 6,
            Cross = 1 << 5,
            Square = 1 << 4,
            DPad_Up = 1 << 3,
            DPad_Down = 1 << 2,
            DPad_Left = 1 << 1,
            DPad_Right = 1 << 0,
            L1 = 1 << 0,
            R1 = 1 << 1,
            Share = 1 << 4,
            Options = 1 << 5,
            L3 = 1 << 6,
            R3 = 1 << 7,
            PS = 1 << 0,
            TouchButton = 1 << 2 - 1
        }

        public DateTime ReportTimeStamp { get; set; }
        public byte LX { get; set; }
        public byte LY { get; set; }
        public byte RX { get; set; }
        public byte RY { get; set; }
        public byte L2 { get; set; }
        public byte R2 { get; set; }
        public bool Triangle { get; set; }
        public bool Circle { get; set; }
        public bool Cross { get; set; }
        public bool Square { get; set; }
        public bool DPad_Up { get; set; }
        public bool DPad_Down { get; set; }
        public bool DPad_Left { get; set; }
        public bool DPad_Right { get; set; }
        public bool L1 { get; set; }
        public bool R1 { get; set; }
        public bool Share { get; set; }
        public bool Options { get; set; }
        public bool L3 { get; set; }
        public bool R3 { get; set; }
        public bool PS { get; set; }

        public Touch Touch1 { get; set; }
        public Touch Touch2 { get; set; }
        public bool TouchButton { get; set; }
        public byte TouchPacketCounter { get; set; }

        public byte FrameCounter { get; set; }
        public byte Battery { get; set; }
        public bool IsCharging { get; set; }

        public short AccelX { get; set; }
        public short AccelY { get; set; }
        public short AccelZ { get; set; }
        public short GyroX { get; set; }
        public short GyroY { get; set; }
        public short GyroZ { get; set; }

        private static byte priorInputReport30 = 0xff;

        /* Constructor */
        public DualShockState()
        {
            LX = 0x80;
            LY = 0x80;
            RX = 0x80;
            RY = 0x80;
            FrameCounter = 255; // null
            TouchPacketCounter = 255; // null
        }

        public DualShockState(DualShockState state)
        {
            ReportTimeStamp = state.ReportTimeStamp;
            LX = state.LX;
            LY = state.LY;
            RX = state.RX;
            RY = state.RY;
            L2 = state.L2;
            R2 = state.R2;
            Triangle = state.Triangle;
            Circle = state.Circle;
            Cross = state.Cross;
            Square = state.Square;
            DPad_Up = state.DPad_Up;
            DPad_Down = state.DPad_Down;
            DPad_Left = state.DPad_Left;
            DPad_Right = state.DPad_Right;
            L1 = state.L1;
            R3 = state.R3;
            Share = state.Share;
            Options = state.Options;
            R1 = state.R1;
            L3 = state.L3;
            PS = state.PS;
            Touch1 = state.Touch1 == null ? null : state.Touch1.Clone();
            Touch2 = state.Touch2 == null ? null : state.Touch2.Clone();
            TouchButton = state.TouchButton;
            TouchPacketCounter = state.TouchPacketCounter;
            FrameCounter = state.FrameCounter;
            Battery = state.Battery;
            IsCharging = state.IsCharging;
            AccelX = state.AccelX;
            AccelY = state.AccelY;
            AccelZ = state.AccelZ;
            GyroX = state.GyroX;
            GyroY = state.GyroY;
            GyroZ = state.GyroZ;
        }

        public void CopyTo(DualShockState state)
        {
            state.ReportTimeStamp = ReportTimeStamp;
            state.LX = LX;
            state.LY = LY;
            state.RX = RX;
            state.RY = RY;
            state.L2 = L2;
            state.R2 = R2;
            state.Triangle = Triangle;
            state.Circle = Circle;
            state.Cross = Cross;
            state.Square = Square;
            state.DPad_Up = DPad_Up;
            state.DPad_Down = DPad_Down;
            state.DPad_Left = DPad_Left;
            state.DPad_Right = DPad_Right;
            state.L1 = L1;
            state.R3 = R3;
            state.Share = Share;
            state.Options = Options;
            state.R1 = R1;
            state.L3 = L3;
            state.PS = PS;
            state.Touch1 = Touch1 == null ? null : Touch1.Clone();
            state.Touch2 = Touch2 == null ? null : Touch2.Clone();
            state.TouchButton = TouchButton;
            state.TouchPacketCounter = TouchPacketCounter;
            state.FrameCounter = FrameCounter;
            state.Battery = Battery;
            state.IsCharging = IsCharging;
            state.AccelX = AccelX;
            state.AccelY = AccelY;
            state.AccelZ = AccelZ;
            state.GyroX = GyroX;
            state.GyroY = GyroY;
            state.GyroZ = GyroZ;
        }

        public DualShockState Clone()
        {
            return new DualShockState(this);
        }

        public static bool IsDefaultState(DualShockState state)
        {
            return (state.LX == _defaultState.LX ||
                        (state.LX >= AnalogDeadZoneMin && state.LX <= AnalogDeadZoneMax)) &&
                    (state.LY == _defaultState.LY ||
                        (state.LY >= AnalogDeadZoneMin && state.LY <= AnalogDeadZoneMax)) &&
                    (state.RX == _defaultState.RX ||
                        (state.RX >= AnalogDeadZoneMin && state.RX <= AnalogDeadZoneMax)) &&
                    (state.RY == _defaultState.RY ||
                        (state.RY >= AnalogDeadZoneMin && state.RY <= AnalogDeadZoneMax)) &&
                    state.L2 == _defaultState.L2 &&
                    state.R2 == _defaultState.R2 &&
                    state.Triangle == _defaultState.Triangle &&
                    state.Circle == _defaultState.Circle &&
                    state.Cross == _defaultState.Cross &&
                    state.Square == _defaultState.Square &&
                    state.DPad_Up == _defaultState.DPad_Up &&
                    state.DPad_Down == _defaultState.DPad_Down &&
                    state.DPad_Left == _defaultState.DPad_Left &&
                    state.DPad_Right == _defaultState.DPad_Right &&
                    state.L1 == _defaultState.L1 &&
                    state.R1 == _defaultState.R1 &&
                    state.Share == _defaultState.Share &&
                    state.Options == _defaultState.Options &&
                    state.L3 == _defaultState.L3 &&
                    state.R3 == _defaultState.R3 &&
                    state.PS == _defaultState.PS &&
                    (state.Touch1 != null && state.Touch1.IsTouched) &&
                    (state.Touch2 != null && state.Touch2.IsTouched);
        }

        public static DualShockState ParseFromDualshockRaw(byte[] data)
        {
            // Validate data
            if (data == null)
                return null;
            if (data[0] != 0x1)
                return null;

            // Create empty state to fill in data
            var result = new DualShockState();

            result.ReportTimeStamp = DateTime.UtcNow; // timestamp with UTC in case system time zone changes

            result.LX = data[1];
            result.LY = data[2];
            result.RX = data[3];
            result.RY = data[4];
            result.L2 = data[8];
            result.R2 = data[9];
            result.Triangle = (data[5] & (byte)VK.Triangle) != 0;
            result.Circle = (data[5] & (byte)VK.Circle) != 0;
            result.Cross = (data[5] & (byte)VK.Cross) != 0;
            result.Square = (data[5] & (byte)VK.Square) != 0;
            result.DPad_Up = (data[5] & (byte)VK.DPad_Up) != 0;
            result.DPad_Down = (data[5] & (byte)VK.DPad_Down) != 0;
            result.DPad_Left = (data[5] & (byte)VK.DPad_Left) != 0;
            result.DPad_Right = (data[5] & (byte)VK.DPad_Right) != 0;

            //Convert dpad into individual On/Off bits instead of a clock representation
            byte dpadState = 0;

            dpadState = (byte)(
            ((result.DPad_Right ? 1 : 0) << 0) |
            ((result.DPad_Left ? 1 : 0) << 1) |
            ((result.DPad_Down ? 1 : 0) << 2) |
            ((result.DPad_Up ? 1 : 0) << 3));

            switch (dpadState)
            {
                case 0: result.DPad_Up = true; result.DPad_Down = false; result.DPad_Left = false; result.DPad_Right = false; break; // ↑
                case 1: result.DPad_Up = true; result.DPad_Down = false; result.DPad_Left = false; result.DPad_Right = true; break; // ↑→
                case 2: result.DPad_Up = false; result.DPad_Down = false; result.DPad_Left = false; result.DPad_Right = true; break; // →
                case 3: result.DPad_Up = false; result.DPad_Down = true; result.DPad_Left = false; result.DPad_Right = true; break; // ↓→
                case 4: result.DPad_Up = false; result.DPad_Down = true; result.DPad_Left = false; result.DPad_Right = false; break; // ↓
                case 5: result.DPad_Up = false; result.DPad_Down = true; result.DPad_Left = true; result.DPad_Right = false; break; // ↓←
                case 6: result.DPad_Up = false; result.DPad_Down = false; result.DPad_Left = true; result.DPad_Right = false; break; // ←
                case 7: result.DPad_Up = true; result.DPad_Down = false; result.DPad_Left = true; result.DPad_Right = false; break; // ↑←
                case 8: result.DPad_Up = false; result.DPad_Down = false; result.DPad_Left = false; result.DPad_Right = false; break; // -
            }

            bool L2Pressed = (data[6] & (byte)VK.L2) != 0;
            bool R2Pressed = (data[6] & (byte)VK.R2) != 0;
            result.L1 = (data[6] & (byte)VK.L1) != 0;
            result.R1 = (data[6] & (byte)VK.R1) != 0;
            result.Share = (data[6] & (byte)VK.Share) != 0;
            result.Options = (data[6] & (byte)VK.Options) != 0;
            result.L3 = (data[6] & (byte)VK.L3) != 0;
            result.R3 = (data[6] & (byte)VK.R3) != 0;
            result.PS = (data[7] & (byte)VK.PS) != 0;
            result.TouchButton = (data[7] & (byte)VK.TouchButton) != 0;

            result.FrameCounter = (byte)(data[7] >> 2);

            // Charging/Battery
            try
            {
                result.IsCharging = (data[30] & 0x10) != 0;
                result.Battery = (byte)((data[30] & 0x0F) * 10);

                if (data[30] != priorInputReport30)
                    priorInputReport30 = data[30];
            }
            catch { throw new InterceptorException("Index out of bounds: battery"); }

            // Accelerometer
            byte[] accel = new byte[6];
            Array.Copy(data, 14, accel, 0, 6);
            result.AccelX = (short)((ushort)(accel[2] << 8) | accel[3]);
            result.AccelY = (short)((ushort)(accel[0] << 8) | accel[1]);
            result.AccelZ = (short)((ushort)(accel[4] << 8) | accel[5]);

            // Gyro
            byte[] gyro = new byte[6];
            Array.Copy(data, 20, gyro, 0, 6);
            result.GyroX = (short)((ushort)(gyro[0] << 8) | gyro[1]);
            result.GyroY = (short)((ushort)(gyro[2] << 8) | gyro[3]);
            result.GyroZ = (short)((ushort)(gyro[4] << 8) | gyro[5]);

            try
            {
                for (int touches = data[-1 + TOUCHPAD_DATA_OFFSET - 1], touchOffset = 0; touches > 0; touches--, touchOffset += 9)
                {
                    //bool touchLeft = (data[1 + TOUCHPAD_DATA_OFFSET + touchOffset] + ((data[2 + TOUCHPAD_DATA_OFFSET + touchOffset] & 0x0F) * 255) >= 1920 * 2 / 5) ? false : true;
                    //bool touchRight = (data[1 + TOUCHPAD_DATA_OFFSET + touchOffset] + ((data[2 + TOUCHPAD_DATA_OFFSET + touchOffset] & 0x0F) * 255) < 1920 * 2 / 5) ? false : true;

                    byte touchID1 = (byte)(data[0 + TOUCHPAD_DATA_OFFSET + touchOffset] & 0x7F);
                    byte touchID2 = (byte)(data[4 + TOUCHPAD_DATA_OFFSET + touchOffset] & 0x7F);
                    bool isTouch1 = (data[0 + TOUCHPAD_DATA_OFFSET + touchOffset] >> 7) != 0 ? false : true; // >= 1 touch detected
                    bool isTouch2 = (data[4 + TOUCHPAD_DATA_OFFSET + touchOffset] >> 7) != 0 ? false : true; // 2 touches detected
                    int currentX1 = data[1 + TOUCHPAD_DATA_OFFSET + touchOffset] + ((data[2 + TOUCHPAD_DATA_OFFSET + touchOffset] & 0x0F) * 255);
                    int currentY1 = ((data[2 + TOUCHPAD_DATA_OFFSET + touchOffset] & 0xF0) >> 4) + (data[3 + TOUCHPAD_DATA_OFFSET + touchOffset] * 16);
                    int currentX2 = data[5 + TOUCHPAD_DATA_OFFSET + touchOffset] + ((data[6 + TOUCHPAD_DATA_OFFSET + touchOffset] & 0x0F) * 255);
                    int currentY2 = ((data[6 + TOUCHPAD_DATA_OFFSET + touchOffset] & 0xF0) >> 4) + (data[7 + TOUCHPAD_DATA_OFFSET + touchOffset] * 16);

                    result.TouchPacketCounter = data[-1 + TOUCHPAD_DATA_OFFSET + touchOffset];
                    result.Touch1 = new Touch(touchID1, isTouch1, currentX1, currentY1);
                    result.Touch2 = new Touch(touchID2, isTouch2, currentX2, currentY2);
                }
            }
            catch { throw new InterceptorException("Index out of bounds: touchpad"); }

            return result;
        }

        public void ConvertToDualshockRaw(ref byte[] data)
        {
            data[1] = LX;
            data[2] = LY;
            data[3] = RX;
            data[4] = RY;
            data[8] = L2;
            data[9] = R2;

            byte data_5 = 0;
            if (Triangle) { data_5 += (byte)VK.Triangle; }
            if (Circle) { data_5 += (byte)VK.Circle; }
            if (Cross) { data_5 += (byte)VK.Cross; }
            if (Square) { data_5 += (byte)VK.Square; }
            if (DPad_Up && !DPad_Down && !DPad_Left && !DPad_Right) { data_5 += 0; } // ↑
            if (DPad_Up && !DPad_Down && !DPad_Left && DPad_Right) { data_5 += 1; } // ↑→
            if (!DPad_Up && !DPad_Down && !DPad_Left && DPad_Right) { data_5 += 2; } // →
            if (!DPad_Up && DPad_Down && !DPad_Left && DPad_Right) { data_5 += 3; } // ↓→
            if (!DPad_Up && DPad_Down && !DPad_Left && !DPad_Right) { data_5 += 4; } // ↓
            if (!DPad_Up && DPad_Down && DPad_Left && !DPad_Right) { data_5 += 5; } // ↓←
            if (!DPad_Up && !DPad_Down && DPad_Left && !DPad_Right) { data_5 += 6; } // ←
            if (DPad_Up && !DPad_Down && DPad_Left && !DPad_Right) { data_5 += 7; } // ↑←
            if (!DPad_Up && !DPad_Down && !DPad_Left && !DPad_Right) { data_5 += 8; } // -
            data[5] = data_5;

            byte data_6 = 0;
            if (L2 > 0) { data_6 += (byte)VK.L2; }
            if (R2 > 0) { data_6 += (byte)VK.R2; }
            if (L1) { data_6 += (byte)VK.L1; }
            if (R1) { data_6 += (byte)VK.R1; }
            if (Share) { data_6 += (byte)VK.Share; }
            if (Options) { data_6 += (byte)VK.Options; }
            if (L3) { data_6 += (byte)VK.L3; }
            if (R3) { data_6 += (byte)VK.R3; }
            data[6] = data_6;

            byte data_7 = 0;
            if (PS) { data_7 += (byte)VK.PS; }
            if (TouchButton) { data_7 += (byte)VK.TouchButton; }
            byte currentFrameCounter = (byte)(data[7] >> 2);
            data_7 += (byte)(currentFrameCounter << 2);
            data[7] = data_7;

            // Accelerometer
            data[14] = (byte)(AccelY >> 8);
            data[15] = (byte)(AccelY & 0xFF);

            data[16] = (byte)(AccelX >> 8);
            data[17] = (byte)(AccelX & 0xFF);

            data[18] = (byte)(AccelZ >> 8);
            data[19] = (byte)(AccelZ & 0xFF);

            // Gyro
            data[20] = (byte)(GyroX >> 8);
            data[21] = (byte)(GyroX & 0xFF);

            data[22] = (byte)(GyroY >> 8);
            data[23] = (byte)(GyroY & 0xFF);

            data[24] = (byte)(GyroZ >> 8);
            data[26] = (byte)(GyroZ & 0xFF);

            // Charging/Battery
            byte data_30 = 0;
            if (IsCharging) data_30 += 0x10;
            if (Battery > 0) data_30 += (byte)(Battery / 10);
            data[30] = data_30;

            // Touches
            try
            {
                for (int touches = data[-1 + TOUCHPAD_DATA_OFFSET - 1], touchOffset = 0; touches > 0; touches--, touchOffset += 9)
                {
                    // Ignore
                    // data[-1 + TOUCHPAD_DATA_OFFSET + touchOffset] = TouchPacketCounter;

                    // >= 1 Finger
                    if (Touch1 != null)
                    {
                        byte oldTouchID = (byte)(data[0 + TOUCHPAD_DATA_OFFSET + touchOffset] & 0x7F);
                        if (Touch1.IsTouched)
                            data[0 + TOUCHPAD_DATA_OFFSET + touchOffset] = oldTouchID;

                        var x = Touch1.X;
                        var xRemain = (int)(x / 255);
                        var xLeft = x - (xRemain * 255);
                        var y = Touch1.Y;
                        var yRemain = (int)(y / 16);
                        var yLeft = y - (yRemain * 16);

                        data[1 + TOUCHPAD_DATA_OFFSET + touchOffset] = (byte)xLeft;
                        data[2 + TOUCHPAD_DATA_OFFSET + touchOffset] = (byte)(xRemain + (yLeft << 4));
                        data[3 + TOUCHPAD_DATA_OFFSET + touchOffset] = (byte)yRemain;
                    }

                    // 2 Fingers
                    if (Touch2 != null)
                    {
                        byte oldTouchID = (byte)(data[4 + TOUCHPAD_DATA_OFFSET + touchOffset] & 0x7F);
                        if (Touch2.IsTouched)
                            data[4 + TOUCHPAD_DATA_OFFSET + touchOffset] = oldTouchID;

                        var x = Touch2.X;
                        var xRemain = (int)(x / 255);
                        var xLeft = x - (xRemain * 255);
                        var y = Touch2.Y;
                        var yRemain = (int)(y / 16);
                        var yLeft = y - (yRemain * 16);

                        data[5 + TOUCHPAD_DATA_OFFSET + touchOffset] = (byte)xLeft;
                        data[6 + TOUCHPAD_DATA_OFFSET + touchOffset] = (byte)(xRemain + (yLeft << 4));
                        data[7 + TOUCHPAD_DATA_OFFSET + touchOffset] = (byte)yRemain;
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Serialize a list of DualShockState to xml file
        /// </summary>
        public static void Serialize(string path, List<DualShockState> list)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<DualShockState>));
            using (TextWriter writer = new StreamWriter(path))
            {
                serializer.Serialize(writer, list);
            }
        }

        /// <summary>
        /// Deserialize a list of DualShockState from xml file
        /// </summary>
        public static List<DualShockState> Deserialize(string path)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(List<DualShockState>));
            using (TextReader reader = new StreamReader(path))
            {
                object obj = deserializer.Deserialize(reader);
                List<DualShockState> list = obj as List<DualShockState>;
                return list;
            }
        }

        /// <summary>
        /// Serialize a list of DualShockState to xml file and remove unchanged properties for smaller file size
        /// </summary>
        public static void SerializeCompressed(string path, List<DualShockState> list, List<string> excludeProperties = null)
        {
            string containerTypeName = "ArrayOfDualShockState";
            Type type = typeof(DualShockState);

            StringBuilder sb = new StringBuilder();

            // Header
            sb.Append($"<?xml version=\"1.0\" encoding=\"utf-8\"?><{containerTypeName}>");

            // Contents
            foreach (var s in list)
            {
                if (excludeProperties == null) excludeProperties = new List<string>();

                XmlDocument doc = new XmlDocument();
                XmlNode typeNode = doc.CreateNode(XmlNodeType.Element, type.Name, string.Empty);
                doc.AppendChild(typeNode);

                // Go through properties
                foreach (PropertyInfo info in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (!info.CanRead || !info.CanWrite) continue;
                    if (excludeProperties.Contains(info.Name)) continue;

                    object templateValue = info.GetValue(_defaultState, null);
                    object changedValue = info.GetValue(s, null);

                    if (changedValue == null) continue;

                    XmlElement propertyElem = doc.CreateElement(info.Name);

                    // Touch
                    if (changedValue is Touch)
                    {
                        Touch t = changedValue as Touch;
                        if (t.IsTouched)
                        {
                            foreach (PropertyInfo t_info in typeof(Touch).GetProperties(BindingFlags.Instance | BindingFlags.Public))
                            {
                                if (!t_info.CanRead || !t_info.CanWrite) continue;

                                object t_value = t_info.GetValue(t, null);

                                XmlElement t_elem = doc.CreateElement(t_info.Name);
                                t_elem.InnerText = t_value.ToString().ToLowerInvariant();
                                propertyElem.AppendChild(t_elem);
                            }

                            // Append property node
                            typeNode.AppendChild(propertyElem);
                        }
                    }
                    // Other properties
                    else
                    {
                        if (templateValue == null || templateValue.Equals(changedValue)) continue;

                        // Bool
                        if (changedValue is bool)
                        {
                            propertyElem.InnerText = changedValue.ToString().ToLowerInvariant();
                        }
                        // DateTime
                        else if (changedValue is DateTime)
                        {
                            propertyElem.InnerText = ((DateTime)changedValue).ToString("o");
                        }
                        // Object
                        else
                        {
                            propertyElem.InnerText = changedValue.ToString();
                        }

                        // Append property node
                        typeNode.AppendChild(propertyElem);
                    }
                }

                using (StringWriter sw = new StringWriter())
                {
                    doc.WriteContentTo(new XmlTextWriter(sw));
                    sb.Append(sw.ToString());
                }
            }

            // Footer
            sb.Append($"</{containerTypeName}>");

            // Write to file
            File.WriteAllText(path, sb.ToString());
        }
    }
}

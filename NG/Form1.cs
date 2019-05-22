using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace NG
{
    public partial class Form1 : Form
    {
        int uavId = 1;
        bool connected = false;

        //Connect to autopilot
        //声明DLL中使用的的函数
        [DllImport("multi-uav.dll")]
        public static extern int mpCreate(int PlaneId);
        [DllImport("multi-uav.dll")]
        public static extern int mpInitLink(int PlaneId, string port);
        [DllImport("multi-uav.dll")]
        public static extern int mpInitFly(int PlaneId, string filename);
        [DllImport("multi-uav.dll")]
        public static extern int mpCloseLink(int PlaneId);
        [DllImport("multi-uav.dll")]
        public static extern int mpCloseFly(int PlaneId);
        [DllImport("multi-uav.dll")]
        public static extern int mpDelete(int PlaneId);
        [DllImport("multi-uav.dll")]
        public static extern int mpGetPlugInData(int PlaneId, ref MPPLUGINDATA data);

        //Other其他
        [DllImport("multi-uav.dll")]
        public static extern int mpResponseStuffServos(int PlaneId, int servo0, int servo1, int servo2, int servo3, ref int pitch, ref int roll, short mode, short comm_mode);
        [DllImport("multi-uav.dll")]
        public static extern int mpGetErrorString(int PlaneId, int err, StringBuilder strBuf, int size);
        [DllImport("multi-uav.dll")]
        public static extern int mpGetWarningString(int PlaneId, Char err, StringBuilder data, short size);
        [DllImport("multi-uav.dll")]
        public static extern int mpGetFatalErrorString(int PlaneId, Char err, string data, short size);
        [DllImport("multi-uav.dll")]
        public static extern int mpStartComTunnel(int PlaneId);
        [DllImport("multi-uav.dll")]
        public static extern int mpStopComTunnel(int PlaneId);
        [DllImport("multi-uav.dll")]
        public static extern int mpTransmitFlash(int PlaneId, ref int progress, string fn, string port);



        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboUavId.SelectedIndex = 0;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (!connected) //Connect连接
            {
                //For this example, select the UAV number from the combo box that matches the UAV#.ini file with the right Comm settings.
                //对于这个例子，从组合框中选择UAV编号，它与UAV的.y.ini文件和右COMM设置相匹配。
                uavId = comboUavId.SelectedIndex + 1;

            }
            //Step 1. Create the UAV
            int result = mpCreate(uavId);

            if (result == 0)
            {//MP-OK - Success
            }
            else if (result == 3072)
            {
                MessageBox.Show("Error 3072 - UAV is in use by another program and could not be created here.");
                mpDelete(uavId);
                return;
            }
            else if (result == 3071) //UAV ID is already created
            {
                //Example using mpGetErrorString to retrieve error message text
                StringBuilder errorText = new StringBuilder();
                mpGetErrorString(uavId, result, errorText, 255);

                MessageBox.Show("ERROR: " + errorText.ToString());
                mpDelete(uavId);
                return;
            }
            //Step 2. Initialize the link to the COM port that the autopilot is connected to
            //A successful call to mpInitLink only means that the port was opened, not that there is a successful communication link to the autopilot.
            //步骤2.初始化到COM端口的链接，自动驾驶仪连接到成功调用MPNITLink只意味着端口被打开
            //而不是有一个成功的与自动驾驶仪的通信链路。
            string empty = ""; //null string = Use the COM port/settings in the selected UAV#.ini file
                               //空字符串=使用选定的无人机.ini文件中的COM端口/设置

            result = mpInitLink(uavId, empty);

            if (result == 0)
            {

            }
            else
            {
                mpDelete(uavId);
                StringBuilder errorText = new StringBuilder();
                errorText.Capacity = 255;
                errorText.EnsureCapacity(255);
                mpGetErrorString(uavId, result, errorText, 255);
                MessageBox.Show("Error initializing port using UAV" + uavId.ToString() + ".ini - Error:" + errorText);
                return;
            }



        }

        public struct MPSTDTELEMETRYDATA
        {
            public int cbSize;                 //ALWAYS init cbSize to the sizeof() this struct before passing to any API.
            public int speed;                //Speed空速 (ft/s).
            public int gpsSpeed; 	            //GPS speed (ft/s)GPS速度.
            public int eTemp; 	            //Unused. Left for compatibility issues.
            public int e; 	            //GPS Longitude (Radians*500000000 - integer scaled radians).
            public int n; 	            //GPS Latitude (Radians*500000000 - integer scaled radians).
            public int alt; 	            //Altitude (-8*ft - integer scaled negative feet).
            public int altDot; 	            //Altitude 1st derivative (-8*ft/s - integer scaled negative feet per second ).
            public int hdg; 	            //Heading (degrees*100 - integer scaled degrees) 0..360 degrees.
            public int err; 	            //Last fatal error code, or 0xff for low battery (see Fatal errors).
            public int status; 	            //Status bit fields. See Status bit field.
            public int status2; 	            //Status bit fields. See Status 2 bit field.
            public int batV; 	           //Main battery voltage (Volts*100 - integer scaled Volts)主电池电压.
            public int sbatV; 	            //Servo battery voltage (Volts*100 - integer scaled Volts)伺服电池电压.
            public int sTh; 	            //Servo throttle position. (FINE-SERVO units mapped to 0..255).
            public int tlmPitch; 	            //Autopilot Pitch (Radians*1024 - integer scaled radians)飞控俯仰.
            public int tlmRoll; 	            //Autopilot Roll (Radians*1024 - integer scaled radians)飞控滚转.
            public int ipStep; 	            //Instruction pointer position.
            public int ipCmd; 	            //Instruction being executed.
            public int patternId; 	           //Pattern invoked if applicable.
            public int failureId; 	            //Failure pattern invoked if applicable. See Control Failures.
            public int targetSpeed_fps; 	            //Target speed (ft/s - feet per second).
            public int targetAlt_ft; 	            //Target altitude (-8*ft - integer scaled negative feet).
            public int targetHeading_deg; 	            //Target heading (degrees*100 - integer scaled degrees) 0..360 degrees.
            public int waypointversion; 	            //Waypoint Version (incremented on waypoint move).
            public int mEvent; 	            //Event warning or non-fatal error status from autopilot.
            public int disableNewOriginSet; 	            //Disable pattern origin movement for certain command conditions.
            public int ownerGcsId; 	            //GCS id of the owner of this UAV ( 0 = no owner ).
            public int gpsAlt; 	            //GPS altitude of the plane (-8*ft - integer scaled negative feet).GPS高度
            public int mpMode; 	            //Mode of the autopilot, shifted left by 4 bits. See GCS Mode.
            public int lrcStatus; 	            //Status of the LRC. See LRC Status Byte.
            public int mp3xStatus; 	            //mp2128 3X board status. See MP3X Status Word
            public int mp3xPitchLeft; 	            //mp2128 3X left board pitch (rad*1024)
            public int mp3xPitchRight; 	            //mp2128 3X right board pitch (rad*1024)
            public int mp3xRollLeft; 	            //mp2128 3X left board roll (rad*1024)
            public int mp3xRollRight; 	            //mp2128 3X right board roll (rad*1024)
            public int warning; 	            //warning error code (see Warnings) 
        }

        public struct MPPLUGINDATA
        {
            public int originaluavid;
            public int pluginuavid;
        }

        private void gMapControl1_Load(object sender, EventArgs e)
        {
                 //************************************主界面GMAP地图的加载项目 * ****************************************
            //GMaps.Instance.PrimaryCache = new MyImageCache();
            gMap.MapProvider = GMap.NET.MapProviders.GoogleChinaSatelliteMapProvider.Instance; ;//选择地图为高德卫星地图(GMap.NET.MapProviders.GoogleChinaMapProvider.Instance);
            //gMap.MapProvider = GMap.NET.MapProviders.GoogleChinaMapProvider.Instance;
            //GMaps.Instance.Mode = AccessMode.ServerAndCache;
            gMap.SetPositionByKeywords("Chengdu, China");//设定初始中心位置为Chengdu，成都坐标为西纬30.67度，东经104.06
            gMap.ShowCenter = false;//不显示中心的红色十字
            gMap.DragButton = System.Windows.Forms.MouseButtons.Left;  //左键拖动地图            
            //设置地图分辨率信息
            gMap.MaxZoom = 18;
            gMap.MinZoom = 3;
            gMap.Zoom = 1;

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            MPSTDTELEMETRYDATA tlmData = new MPSTDTELEMETRYDATA();

            //Must set cbSize to the size of the structure before using it in mpGetStandardTelemetry
            tlmData.cbSize = Marshal.SizeOf(tlmData);

            label5.Text = tlmData.tlmRoll.ToString("");
           
            label6.Text = tlmData.tlmPitch.ToString("");

          
            

        }
    }
}

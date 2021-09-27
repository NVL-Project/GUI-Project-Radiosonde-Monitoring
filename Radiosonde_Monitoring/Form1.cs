using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Web;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms.ToolTips;
using GMap.NET.CacheProviders;
using ZedGraph;



namespace Radiosonde_Monitoring
{
    public partial class Form1 : Form
    {
        int flag_map;
        int TickStart1;
        int TickStart2;
        int TickStart3;
        int TickStart4;
        int TickStart5;

        String time;
        String sec;
        String day;
        String date;

        double Lat;
        double Long;
        double preLat;
        double preLong;
        double gcslat;
        double gcslong;

        bool set;
        bool mapFlag;
        GMarkerGoogle balon;
        List<PointLatLng> list = new List<PointLatLng>();

        public Form1()
        {
            Thread t = new Thread(new ThreadStart(SplashStart)); // deklarasi t sebagai Thread
            t.Start(); // memulai fungsi t
            Thread.Sleep(1000); // sleep 1000mS
            InitializeComponent();
            t.Abort(); // menggugurkan fungsi t   
            timer2.Start(); // menjalankan timer 2
        }

        public void SplashStart()
        {
            Application.Run(new Form2()); // menjalankan Form2
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            String[] portList = System.IO.Ports.SerialPort.GetPortNames(); // membuat variable portList untuk GetPortName pada serial port
            foreach (String portName in portList)
                comboBox1.Items.Add(portName); // add portName pada comboBox1
            comboBox1.Text = comboBox1.Items[comboBox1.Items.Count - 1].ToString(); //drop down saat mendeteksi serial port baru 

            MAAP(); // memanggil deklarasi (MAAP)
            grafik(); // memanggil deklarasi (grafik)
            

        }

        private void MAAP()
        {

            gmap.DragButton = MouseButtons.Right;// geser posisi maap dengan klik kanan pada mouse
            gmap.MapProvider = GMap.NET.MapProviders.BingSatelliteMapProvider.Instance; // set provider pada maap
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache; // set maap supaya bisa digunakan online maupun offline
            gmap.MinZoom = 1; // minimal zoom = 1
            gmap.MaxZoom = 50; // maksimal zoom = 50
            gmap.Zoom = 20; // set zoom = 20

        }

        private void grafik()
        {
            GraphPane panel1 = zedGraphControl1.GraphPane; //membuat deklarsi panel1 untuk menjalankan panel pada zedGraphControl1
            panel1.Title.Text = "Suhu"; // menulist text (Suhu) pada panel1
            panel1.Title.FontSpec.Size = 20; // ukuran title pada panel1
            RollingPointPairList list1 = new RollingPointPairList(500);
            LineItem curve1 = panel1.AddCurve("value", list1, Color.Blue, SymbolType.None);

            GraphPane panel2 = zedGraphControl2.GraphPane; //membuat deklarsi panel1 untuk menjalankan panel pada zedGraphControl2
            panel2.Title.Text = "Kelembaban"; // menulis text (Kelembaban) pada panel2
            panel2.Title.FontSpec.Size = 20; // ukuran title pada panel2
            RollingPointPairList list2 = new RollingPointPairList(500);
            LineItem curve2 = panel2.AddCurve("value", list2, Color.Blue, SymbolType.None);

            GraphPane panel3 = zedGraphControl3.GraphPane; //membuat deklarsi panel1 untuk menjalankan panel pada zedGraphControl3
            panel3.Title.Text = "Tekanan"; // menulis text (Tekanan) pada panel3
            panel3.Title.FontSpec.Size = 20; // ukuran title pada panel3
            RollingPointPairList list3 = new RollingPointPairList(500);
            LineItem curve3 = panel3.AddCurve("value", list3, Color.Blue, SymbolType.None);

            GraphPane panel4 = zedGraphControl4.GraphPane; //membuat deklarsi panel1 untuk menjalankan panel pada zedGraphControl4
            panel4.Title.Text = "Arah Angin"; // menulis text (Arah Angin) pada panel4
            panel4.Title.FontSpec.Size = 20; // ukuran title pada panel4
            RollingPointPairList list4 = new RollingPointPairList(500);
            LineItem curve4 = panel4.AddCurve("value", list4, Color.Blue, SymbolType.None);

            GraphPane panel5 = zedGraphControl5.GraphPane; //membuat deklarsi panel1 untuk menjalankan panel pada zedGraphControl5
            panel5.Title.Text = "Kecepatan Angin"; // menulis text (Kecepatan Angin) pada panel5
            panel5.Title.FontSpec.Size = 20; // ukuran title pada panel5
            RollingPointPairList list5 = new RollingPointPairList(500);
            LineItem curve5 = panel5.AddCurve("value", list5, Color.Blue, SymbolType.None);

            panel1.XAxis.Scale.Min = 0; // minimal scala X pada panel1 = 0
            panel1.XAxis.Scale.Max = 10; // maksimal scala X pada panel1 = 10
            panel1.YAxis.Scale.Min = -100; // minimal scala Y pada panel1 = -100
            panel1.YAxis.Scale.Max = 100; // maksimal scala Y pada panel1 = 100
            zedGraphControl1.AxisChange(); // ganti Axix pada zedGraphControl1

            panel2.XAxis.Scale.Min = 0; // minimal scala X pada panel2 = 0
            panel2.XAxis.Scale.Max = 10; // maksimal scala X pada panel2 = 10
            panel2.YAxis.Scale.Min = 0; // minimal scala Y pada panel2 = 0
            panel2.YAxis.Scale.Max = 100; // maksimal scala Y pada panel2 = 100
            zedGraphControl2.AxisChange(); // ganti Axix pada zedGraphControl2

            panel3.XAxis.Scale.Min = 0; // minimal scala X pada panel3 = 0
            panel3.XAxis.Scale.Max = 10; // maksimal scala X pada panel3 = 10
            panel3.YAxis.Scale.Min = 0; // minimal scala Y pada panel3 = 0
            panel3.YAxis.Scale.Max = 10000; // maksimal scala Y pada panel3 = 10000
            zedGraphControl3.AxisChange(); // ganti Axix pada zedGraphControl3

            panel4.XAxis.Scale.Min = 0; // minimal scala X pada panel4 = 0
            panel4.XAxis.Scale.Max = 10; // maksimal scala X pada panel4 = 10
            panel4.YAxis.Scale.Min = 0;  // minimal scala Y pada panel4 = 0
            panel4.YAxis.Scale.Max = 360; // maksimal scala Y pada panel4 = 360
            zedGraphControl4.AxisChange(); // ganti Axix pada zedGraphControl4

            panel5.XAxis.Scale.Min = 0; // minimal scala X pada panel5 = 0
            panel5.XAxis.Scale.Max = 10; // maksimal scala X pada panel5 = 10
            panel5.YAxis.Scale.Min = 0.01; // minimal scala Y pada panel5 = 0.01
            panel5.YAxis.Scale.Max = 20; // maksimal scala Y pada panel5 = 20
            zedGraphControl5.AxisChange(); // ganti Axix pada zedGraphControl5

            TickStart1 = Environment.TickCount;
            TickStart2 = Environment.TickCount;
            TickStart3 = Environment.TickCount;
            TickStart4 = Environment.TickCount;
            TickStart5 = Environment.TickCount;

        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult confirm = MessageBox.Show("Anda yakin Keluar dari aplikasi?", "Konfigurasi", MessageBoxButtons.YesNo); // konfirmasi saat tombol close ditekan
            if (confirm == DialogResult.No) e.Cancel = true; // konfirmasi (No) untuk membatalkan 
            else if (serialPort1.IsOpen) serialPort1.Close(); // menutup serial port


        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear(); // hapus item pada comboBox
            string[] myPort; // membuat variabel string di myPort
            myPort = System.IO.Ports.SerialPort.GetPortNames(); // mencari  serial port baru
            comboBox1.Items.AddRange(myPort); // menampilkan serial port baru
        }

        private void Connect_Click(object sender, EventArgs e)
        {
            if (Connect.Text == "CONNECT")
            {
                try
                {
                    serialPort1.PortName = comboBox1.Text; // set port name pada comboBox 1
                    serialPort1.BaudRate = Int32.Parse(comboBox2.Text); // set Baud Rate pada comboBox 2
                    serialPort1.NewLine = "\r\n"; // membuat baris baru
                    serialPort1.Open(); // membuka serial port
                    toolStripStatusLabel1.Text = serialPort1.PortName + " Is Connect"; // menampilkan text  di toolStripStatusLabel1 (port yang terdeteksi + Is Connect) 
                    Connect.Text = "CLOSE"; // membuat text (CLOSE) pada button Connect
                    
                }
                catch (Exception ex)
                {
                    toolStripStatusLabel1.Text = "ERROR:" + ex.Message.ToString(); // menampilkan pesan box (ERROR)
                }
            }
            else
            {
                serialPort1.Close(); // menutup serial port
                Connect.Text = "CONNECT"; // memberi nama pada button Connect (CONNECT)
                toolStripStatusLabel1.Text = serialPort1.PortName + " is closed."; // menampilkan text di toolStripStatusLabel1 (port yang terdeteksi + Is closed)
            }
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            if (serialPort1.IsOpen == true)
            {
                String receivedMsg = serialPort1.ReadLine(); // membaca data yang dikirim melalui serial port
                Tampilkan(receivedMsg); // memanggil deklarsi Tampilkan
            }
        }

        private delegate void TampilkanDelegate(object item);

        private void Tampilkan(object item)
        {
            
            if (InvokeRequired)
                // This is a worker thread so delegate the task.
                listBox1.Invoke(new TampilkanDelegate(Tampilkan), item); // invoke data serial port
            else
            {
                // This is the UI thread so perform the task.
                listBox1.Items.Add(label20.Text + item); // menampilkan data pada lebel20 dan data yang dikirim melalui serial port
                listBox1.SelectedIndex = listBox1.Items.Count - 1; // data yang lama akan dikeser keatas atau setiap data baru barih akan mengikuti data tersebut
                splitData(item); // memanggil deklarai splitData
            }
            
        }

        private void Date()
        {
            label20.Text = time + sec + day + date; // menampilkan jam, detik, tanggal/bulan/tahun pada label 20
            timer2.Start(); // menjalankan timer 2
        }
        private void splitData(object item)
        {
            String[] data = item.ToString().Split(','); // membaca data sring dan membagi data pada kondisi (,)

            String Battery = data[1]; // membuat variable string Battery untuk menampilkan data[1]
            double D_Battery = Convert.ToDouble(Battery); // mengonkonversi variable Baterry menjadi double dan ditempatka di didalam variable D_Battery
            lbDigitalMeter1.Value = D_Battery; // membaca variable D_Baterry dan menampilkan nilainya ke dalam lbDigitalMeter1

            textBox1.Text = data[2]; // textbox untuk data suhu
            textBox2.Text = data[3]; // textbox untuk data kelembaban
            textBox3.Text = data[4]; // textbox untuk data tekanan udara
            textBox4.Text = data[5]; // textbox untuk data arah angin
            textBox5.Text = data[6]; // textbox untuk data kecepatan angin
            textBox6.Text = data[7]; // textbox untuk data ketinggian
            textBox7.Text = data[8]; // textbox untuk data latitude
            textBox8.Text = data[9]; // textbox untuk data longitude

            timer1.Start(); // menjalankan timer 1

            

                Lat =(Convert.ToDouble(data[8].ToString())); // konversi data[8] dari string menjadi double dan dimasukkan ke variable Lat
                Long = (Convert.ToDouble(data[9].ToString())); // konversi data[9] dari string menjadi double dan dimasukkan ke vaeriable Long
                if (mapFlag == true)
                {
                    if (Lat != null && Long != null)
                    {
                        PointLatLng start = new PointLatLng(preLat, preLong); // membuat deklarasi (start) untuk plot maap
                        PointLatLng end = new PointLatLng(Lat, Long); // membuat deklarasi (end) untuk plot maap
                        list.Add(new PointLatLng(preLat, preLong)); //add data preLat dan preLong
                        list.Add(new PointLatLng(Lat, Long)); // add data Lat dan Long

                        GMapRoute r = new GMapRoute(list, "My route"); // membuat deklarasi (r) untuk set route
                        GMapOverlay routeOverlay = new GMapOverlay("route"); 
                        balon.Position = new PointLatLng(Lat, Long); // set posisi Lat dan Long
                        routeOverlay.Routes.Add(r);
                        gmap.Overlays.Add(routeOverlay);
                        balon.ToolTipText = Lat + "," + Long; // menampilkan data Lat Long pada posisi balon
                        gmap.UpdateRouteLocalPosition(r); // update lokasi maap
                        list.RemoveRange(0, 2);
                        flag_map++;
                    }
                }

                preLat = Lat; // menjadikan variable preLat sama dengan Lat
                preLong = Long; // menjadikan variable preLong sama dengan Long
        }

        private void Draw( string SH, string KEL, string TEKN, string ARAH_A, string KEC_A )
        {
            double insetpoin1; // membuat variable double pada insetpoin1
            double insetpoin2; // membuat variable double pada insetpoin2
            double insetpoin3; // membuat variable double pada insetpoin3
            double insetpoin4; // membuat variable double pada insetpoin4
            double insetpoin5; // membuat variable double pada insetpoin5

            double.TryParse(SH, out insetpoin1); // membuat variable double dengan SH di konversi ke insertpoin1 sebagai output
            double.TryParse(KEL, out insetpoin2); // membuat variable double dengan SH di konversi ke insertpoin2 sebagai output
            double.TryParse(TEKN, out insetpoin3); // membuat variable double dengan SH di konversi ke insertpoin3 sebagai output
            double.TryParse(ARAH_A, out insetpoin4); // membuat variable double dengan SH di konversi ke insertpoin4 sebagai output
            double.TryParse(KEC_A, out insetpoin5); // membuat variable double dengan SH di konversi ke insertpoin5 sebagai output

            if (zedGraphControl1.GraphPane.CurveList.Count <= 0)
                return; // mengulangi program 
            if (zedGraphControl2.GraphPane.CurveList.Count <= 0)
                return; // mengulangi program
            if (zedGraphControl3.GraphPane.CurveList.Count <= 0)
                return; // mengulangi program
            if (zedGraphControl4.GraphPane.CurveList.Count <= 0)
                return; // mengulangi program
            if (zedGraphControl5.GraphPane.CurveList.Count <= 0)
                return; // mengulangi program


            LineItem Curve1 = zedGraphControl1.GraphPane.CurveList[0] as LineItem; // deklarasi Curve1 sebagai LineItem pada zedGraphControl1
            LineItem Curve2 = zedGraphControl2.GraphPane.CurveList[0] as LineItem; // deklarasi Curve1 sebagai LineItem pada zedGraphControl2
            LineItem Curve3 = zedGraphControl3.GraphPane.CurveList[0] as LineItem; // deklarasi Curve1 sebagai LineItem pada zedGraphControl3
            LineItem Curve4 = zedGraphControl4.GraphPane.CurveList[0] as LineItem; // deklarasi Curve1 sebagai LineItem pada zedGraphControl4
            LineItem Curve5 = zedGraphControl5.GraphPane.CurveList[0] as LineItem; // deklarasi Curve1 sebagai LineItem pada zedGraphControl5

            if (Curve1 == null)
                return; // mengulangi program
            if (Curve2 == null)
                return; // mengulangi program
            if (Curve3 == null)
                return; // mengulangi program
            if (Curve4 == null)
                return; // mengulangi program
            if (Curve5 == null)
                return; // mengulangi program

            IPointListEdit list1 = Curve1.Points as IPointListEdit; //deklarasi list1 sebagai IPointListEdit
            IPointListEdit list2 = Curve2.Points as IPointListEdit; //deklarasi list2 sebagai IPointListEdit
            IPointListEdit list3 = Curve3.Points as IPointListEdit; //deklarasi list3 sebagai IPointListEdit
            IPointListEdit list4 = Curve4.Points as IPointListEdit; //deklarasi list4 sebagai IPointListEdit
            IPointListEdit list5 = Curve5.Points as IPointListEdit; //deklarasi list5 sebagai IPointListEdit

            if (list1 == null)
                return; // mengulangi program
            if (list2 == null)
                return; // mengulangi program
            if (list3 == null)
                return; // mengulangi program
            if (list4 == null)
                return; // mengulangi program
            if (list5 == null)
                return; // mengulangi program

            double time1 = (Environment.TickCount - TickStart1) / 1000.0; // membuat variable double untuk sebagai timer 1
            double time2 = (Environment.TickCount - TickStart2) / 1000.0; // membuat variable double untuk sebagai timer 2
            double time3 = (Environment.TickCount - TickStart3) / 1000.0; // membuat variable double untuk sebagai timer 3
            double time4 = (Environment.TickCount - TickStart4) / 1000.0; // membuat variable double untuk sebagai timer 4
            double time5 = (Environment.TickCount - TickStart5) / 1000.0; // membuat variable double untuk sebagai timer 5

            // add list insertpoint1 - 5
            list1.Add(time1, Convert.ToDouble(insetpoin1));
            list2.Add(time2, Convert.ToDouble(insetpoin2));
            list3.Add(time3, Convert.ToDouble(insetpoin3));
            list4.Add(time4, Convert.ToDouble(insetpoin4));
            list5.Add(time5, Convert.ToDouble(insetpoin5));

            // scale x pada zedGraphControl1.GraphPane
            Scale xScale1 = zedGraphControl1.GraphPane.XAxis.Scale;
            Scale xScale2 = zedGraphControl2.GraphPane.XAxis.Scale;
            Scale xScale3 = zedGraphControl3.GraphPane.XAxis.Scale;
            Scale xScale4 = zedGraphControl4.GraphPane.XAxis.Scale;
            Scale xScale5 = zedGraphControl5.GraphPane.XAxis.Scale;

            // scale y pada zedGraphControl1.GraphPane
            Scale yScale1 = zedGraphControl1.GraphPane.YAxis.Scale;
            Scale yScale2 = zedGraphControl2.GraphPane.YAxis.Scale;
            Scale yScale3 = zedGraphControl3.GraphPane.YAxis.Scale;
            Scale yScale4 = zedGraphControl4.GraphPane.YAxis.Scale;
            Scale yScale5 = zedGraphControl5.GraphPane.YAxis.Scale;

            if (time1 > xScale1.Max - xScale1.MajorStep)
            {
                xScale1.Max = time1 + xScale1.MajorStep; // maksimal scale x 
                xScale1.Min = xScale1.Max - 10; // minimal scale x
            }
            if (time2 > xScale2.Max - xScale2.MajorStep)
            {
                xScale2.Max = time2 + xScale2.MajorStep; // maksimal scale x
                xScale2.Min = xScale2.Max - 10; // minimal scale x
            }
            if (time3 > xScale3.Max - xScale3.MajorStep)
            {
                xScale3.Max = time3 + xScale3.MajorStep; // maksimal scale x
                xScale3.Min = xScale3.Max - 10; // minimal scale x
            }
            if (time4 > xScale4.Max - xScale4.MajorStep)
            {
                xScale4.Max = time4 + xScale4.MajorStep; // maksimal scale x
                xScale4.Min = xScale4.Max - 10; // minimal scale x
            }
            if (time5 > xScale5.Max - xScale5.MajorStep)
            {
                xScale5.Max = time5 + xScale5.MajorStep; // maksimal scale x
                xScale5.Min = xScale5.Max - 10; // minimal scale x
            }

            // ganti Axis zedGraphControl
            zedGraphControl1.AxisChange();
            zedGraphControl2.AxisChange();
            zedGraphControl3.AxisChange();
            zedGraphControl4.AxisChange();
            zedGraphControl5.AxisChange();

            //validate zedGraphControl
            zedGraphControl1.Invalidate();
            zedGraphControl2.Invalidate();
            zedGraphControl3.Invalidate();
            zedGraphControl4.Invalidate();
            zedGraphControl5.Invalidate();
        }

        private void Request_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen == true)
            {
                if (Request.Text == "RUN")
                    try
                    {
                        serialPort1.WriteLine("Run"); // mengirim data serial (Run)
                        Request.Text = ("STOP"); // membuat text pada button Request (STOP)
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Tidak dapat mengirim" + ex.Message.ToString()); // menampilkan pesan box (Tidak dapat mengirim)
                    }
                else
                {
                    try
                    {
                        serialPort1.WriteLine("Stop"); // mengirim data serial port (Stop)
                        Request.Text = ("RUN"); // membuat text pada button Request (RUN)
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Tidak dapat mengirim" + ex.Message.ToString()); // menampilkan pesan box (Tidak dapat mengirim)
                    }
                }
            }
            else
                MessageBox.Show("Serial belum terkoneksi"); // menampilkan pesan box (Serial belum terkoneksi)


        }

        private void SaveFile_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen == true)
            {
                Date(); //memanggil deklarasi (Date)
                SaveFileDialog saveFileDialog1 = new SaveFileDialog(); // membuat variable untuk menyimpan data baru
                saveFileDialog1.Filter = "Text Dokument(*.txt) | *.txt"; // membuat data dengan format txt
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (Stream file = File.Open(saveFileDialog1.FileName, FileMode.CreateNew))
                        using (StreamWriter list = new StreamWriter( file))
                        {
                            foreach (var item in listBox1.Items)
                            {
                                list.WriteLine(item); // membaca data pada item
                                
                            }
                        }
                    }
                    catch { }

                    toolStripStatusLabel2.Text = "Data berhasil disimpan"; // menampilkan text (Data berhasil disimpan) pada toolStripStatusLabel2.Text
                }
                
            }
            else
                MessageBox.Show("Serial belum terkoneksi"); // menampilkan pesan box (Serial belum terkoneksi)
        }

        private void Data_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen == true)
            {
                textBox9.Text = textBox7.Text; //menampilkan data text pada textBox9 dari textBox7
                textBox10.Text = textBox8.Text; //menampilkan data text pada textBox10 dari textBox8
            }
            else
                MessageBox.Show("Serial belum terkoneksi"); // menampilkan pesan box (Serial belum terkoneksi)
        }

        private void SetGCS_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen == true)
            {
                try
                {
                    gmap.Overlays.Clear(); //hapus plot maap
                    gcslat = Convert.ToDouble(textBox9.Text); // convert data textBox9 menjadi double
                    gcslong = Convert.ToDouble(textBox10.Text); // convert data textBox10 menjadi double
                    gmap.Position = new PointLatLng(Lat, Long);
                    gmap.Zoom = 18; // set zoom maap = 18
                    GMapOverlay markerOverlay = new GMapOverlay("markers");
                    GMarkerGoogle markergcs = new GMarkerGoogle(new PointLatLng(gcslat, gcslong), new Bitmap(@"E:\\KULIAH\Semester4\\Antar Muka\\Program\\P13\\Radiosonde_Monitoring\\antena.png"));
                    markergcs.ToolTipMode = MarkerTooltipMode.Always;
                    markergcs.ToolTipText = gcslat + "," + gcslong;
                    markerOverlay.Markers.Add(markergcs);
                    gmap.Overlays.Add(markerOverlay);
                    gmap.UpdateMarkerLocalPosition(markergcs);
                }
                catch
                {
                    MessageBox.Show("Data belum dimasukkan");
                }
            }
            else
                MessageBox.Show("Serial belum terkoneksi"); // menampilkan pesan box (Serial belum terkoneksi)
        }


        private void Track_Click(object sender, EventArgs e)
        {
            GMapOverlay markerOverlay = new GMapOverlay("markers");
            balon = new GMarkerGoogle(new PointLatLng(gcslat, gcslong), new Bitmap(@"E:\\KULIAH\Semester4\\Antar Muka\\Program\\P13\\Radiosonde_Monitoring\\winds_white.png"));
            balon.ToolTipText = Lat + "," + Long;
            balon.ToolTipMode = MarkerTooltipMode.Always;
            markerOverlay.Markers.Add(balon);
            gmap.Overlays.Add(markerOverlay);

            if (mapFlag == false)
            {
                mapFlag = true;
                Track.Text = "Stop";
            }
            else
            {
                mapFlag = false;
                Track.Text = "Track";
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
                Draw(textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text, textBox5.Text); // memangil deklarasi Draw dan memasukkan textBox1 - textBox5
                label3.Text = "Suhu " + textBox1.Text + " °C"; // menulis text pada label3 (Suhu + nilai pada textBox1)
                label4.Text = "Kelembaban " + textBox2.Text + " %"; // menulis text pada label4 (Kelembaban + nilai pada textBox2)
                label5.Text = "Tekanan " + textBox3.Text + " mBar"; // menulis text pada label5 (Tekanan + nilai pada textBox3)
                label6.Text = "Arah Angin " + textBox4.Text + " °"; // menulis text pada label6 (Arah Angin + nilai pada textBox4)
                label7.Text = "Kecepatan Angin " + textBox5.Text + " m/s"; // menulis text pada label7 (Kecepatan Angin + nilai pada textBox5)
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            time = DateTime.Now.ToString("HH:mm"); // menampilkan jam dan menit pada variable time
            sec = DateTime.Now.ToString(":ss/ "); // menampilkan detik pada variable sec
            day = DateTime.Now.ToString("dddd/ "); // menampilkan hari pada variable day
            date = DateTime.Now.ToString("MM/dd/yyy/ "); // menampilkan tanggal/bulan/tahun pada variable date

            label20.Text = time + sec + day + date; // menampilkan data pada variable time,sec,day, dan date pada lebel20
        }

    }
}

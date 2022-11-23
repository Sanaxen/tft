using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace tft
{
    public partial class interactivePlot : Form
    {
        private void webView21_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (webView21.CoreWebView2 != null)
            {
                //Web画面からVB/C＃へのホストオブジェクトにアクセスする必要がなければ
                webView21.CoreWebView2.Settings.AreHostObjectsAllowed = false;

                //Webコンテンツ(JavaScript)からVB／C＃側へのメッセージを処理する必要がなければ
                //webView21.CoreWebView2.Settings.IsWebMessageEnabled = false;

                //Web画面でJavaScriptを使用したくなければ
                //webView21.CoreWebView2.Settings.IsScriptEnabled = false;

                //alertやpromptなどのダイアログを表示したくなければ
                webView21.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = false;
            }
        }
        public interactivePlot()
        {
            InitializeComponent();
            InitializeAsync();
            webView21.NavigationCompleted += webView21_NavigationCompleted;
        }
        async void InitializeAsync()
        {
            try
            {
                await webView21.EnsureCoreWebView2Async(null);
            }
            catch (Exception)
            {
                MessageBox.Show("The WebView2 runtime may not be installed.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //this.Close();
            }
        }
        public static System.Drawing.Image CreateImage(string filename)
        {
            System.IO.FileStream fs = new System.IO.FileStream(
                filename,
                System.IO.FileMode.Open,
                System.IO.FileAccess.Read);
            System.Drawing.Image img = System.Drawing.Image.FromStream(fs);
            fs.Close();
            return img;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Bitmap bmp = new Bitmap(pictureBox1.Image);
                Clipboard.SetImage(bmp);

                bmp.Dispose();
                TopMost = true;
                TopMost = false;
            }
            catch
            {

            }
        }
    }
}

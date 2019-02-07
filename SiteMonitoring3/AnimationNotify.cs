using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;  // Win32 함수 사용

namespace SiteMonitoring3
{
    public partial class AnimationNotify : Form
    {
        public enum NotifyState
        {
            Hidden = 0,
            Visible = 1,
            Appear = 2,
            DisAppear = 3
        }

        delegate void notify_popup(); // 공지창 출력

        private NotifyState notifyState = NotifyState.Hidden;
        private Bitmap backgroundImage;           // 배경화면
        private Rectangle screenRect;             // 공지창 출력 영역
        private Timer timer = null;               // 타이머

        private int nShowCount = 0;                    // 화면 출력될때 애니메이션 시간 간격
        private int nShowIncrement = 0;                // 화면이 출력될때 애니메이션당 증가치
        private int nVisibleCount = 0;                 // 화면에 공지창 출력 시간 간격
        private int nHideCount = 0;                    // 화면 사라질때 애니메이션 시간 간격
        private int nHideDecrement = 0;                // 화면 사라질때 애니메이션 횟수

        int nShowTime;
        int nStayTime;
        int nHideTime;

        public string title = "";
        public string URL = "about:blank";

        public AnimationNotify()
        {
            InitializeComponent();

            // 윈도우 초기화
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Minimized;
            base.Show();
            base.Hide();
            WindowState = FormWindowState.Normal;
            TopMost = true;
            MaximizeBox = false;
            MinimizeBox = false;
            ControlBox = false;
            
            // 타이머 설정
            timer = new Timer();
            timer.Enabled = true;
            timer.Tick += new EventHandler(OnTimer);
            
            // 출력될 배경 이미지 설정
            this.backgroundImage = new Bitmap(Properties.Resources.Notify);
            this.Width = backgroundImage.Width;       // 출력 창의 폭 설정 
            this.Height = backgroundImage.Height;     // 출력 창의 높이 설정

            //나타나는 시간, 유지되는 시간, 사라지는 시간 설정
            this.nShowTime = 500;
            this.nStayTime = 4000;
            this.nHideTime = 500;
        }
        /// <summary>
        /// 타이머 이벤트 메소드
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ea"></param>
        protected void OnTimer(Object obj, EventArgs ea)
        {
            switch (this.notifyState)
            {
                case NotifyState.Appear:
                    if (this.Height < this.backgroundImage.Height)
                    {
                        this.SetBounds(this.Left, this.Top - this.nShowIncrement,
                                       this.Width, this.Height + this.nShowIncrement);
                    }
                    else
                    {
                        timer.Stop();
                        this.Height = this.backgroundImage.Height;
                        timer.Interval = this.nVisibleCount;
                        this.notifyState = NotifyState.Visible;
                        timer.Start();
                    }
                    break;

                case NotifyState.Visible:
                    timer.Stop();
                    timer.Interval = this.nHideCount;

                    this.notifyState = NotifyState.DisAppear;

                    timer.Start();
                    break;

                case NotifyState.DisAppear:
                    if (this.Top < this.screenRect.Bottom)
                        this.SetBounds(this.Left, this.Top + this.nHideDecrement, this.Width, this.Height - this.nHideDecrement);
                    else
                    {
                        this.Hide();
                        this.Dispose();
                    }
                    break;
            }

        }

        protected override void OnPaintBackground(PaintEventArgs pea)
        {
            Graphics grathic = pea.Graphics;
            grathic.PageUnit = GraphicsUnit.Pixel;

            Graphics offscreen;
            Bitmap bmp;

            bmp = new Bitmap(this.backgroundImage.Width, this.backgroundImage.Height);
            offscreen = Graphics.FromImage(bmp);

            // 바탕화면 그리기
            offscreen.DrawImage(this.backgroundImage, 0, 0, this.backgroundImage.Width, this.backgroundImage.Height);

            grathic.DrawImage(bmp, 0, 0);
        }
        /// <summary>
        /// 창 숨기기
        /// </summary> 
        public new void Hide()
        {
            if (this.notifyState != NotifyState.Hidden)
            {
                timer.Stop();
                this.notifyState = NotifyState.Hidden;
                base.Hide();
            }
        }

        /// <summary>
        /// 화면에 Notify 창을 호출하기위해 Win32 API 사용
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="nCmdShow"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern Boolean ShowWindow(IntPtr hWnd, Int32 nCmdShow);

        /*
        public void NotifyPopup()
        {
            if (this.InvokeRequired)
            {
                notify_popup notify = new notify_popup(Show);
                this.Invoke(notify);
            }
            else
                Show();
        }*/
        
        /// <summary>
        /// 공지창 출력
        /// </summary>
        public void Show()
        {
            if (this.InvokeRequired)
            {
                notify_popup dele = new notify_popup(Show);
                this.Invoke(dele);
            }
            else
            {
                //링크 걸기
                linkLabel1.Text = title;
                linkLabel1.Links.Add(0, linkLabel1.Text.Length, URL);

                this.screenRect = Screen.GetWorkingArea(this.screenRect);
                this.nVisibleCount = nStayTime;

                // 화면에 창이 출력될때 애니메이션 설정
                int nCount = 0;             // 화면 갱신 횟수

                if (nShowTime > 10)
                {
                    nCount = Math.Min((nShowTime / 10), this.backgroundImage.Height);
                    this.nShowCount = nShowTime / nCount;    // 화면 출력될때 애니메이션 횟수
                    this.nShowIncrement = this.backgroundImage.Height / nCount;  // 한번 화면이 갱신될때 증가치
                }
                else
                {
                    this.nShowCount = 10;
                    this.nShowIncrement = this.backgroundImage.Height;
                }

                // 화면이 닫힐때 애니메이션 설정
                if (nHideTime > 10)
                {
                    nCount = Math.Min((nHideTime / 10), this.backgroundImage.Height);
                    this.nHideCount = nHideTime / nCount;
                    this.nHideDecrement = this.backgroundImage.Height / nCount;
                }
                else
                {
                    this.nHideCount = 10;
                    this.nHideDecrement = this.backgroundImage.Height;
                }

                // Hidden -> Appear -> Visible -> DisAppear -> Hidden 순으로 화면 변화
                switch (this.notifyState)
                {
                    case NotifyState.Hidden:
                        this.notifyState = NotifyState.Appear;
                        this.SetBounds(this.screenRect.Right - this.backgroundImage.Width - 10, this.screenRect.Bottom - 1, this.backgroundImage.Width, 0);
                        timer.Interval = this.nShowCount;  // 타이머 시간 설정
                        timer.Start();
                        // 바탕 화면에 창 출력
                        ShowWindow(this.Handle, 4); // Win32 API 함수 호출
                        break;

                    case NotifyState.Visible:
                        timer.Stop();
                        timer.Interval = this.nVisibleCount;
                        timer.Start();
                        Refresh();
                        break;

                    case NotifyState.Appear:
                        Refresh();
                        break;

                    case NotifyState.DisAppear:
                        timer.Stop();
                        this.notifyState = NotifyState.Visible;
                        this.SetBounds(this.screenRect.Right - this.backgroundImage.Width - 10, this.screenRect.Bottom - this.backgroundImage.Height - 1, this.backgroundImage.Width, this.backgroundImage.Height);
                        timer.Interval = this.nVisibleCount;
                        timer.Start();
                        Refresh();
                        break;
                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            timer.Stop();
            OnTimer(sender, e);
            openWebBrowser();
        }

        //웹브라우저 실행
        private void openWebBrowser()
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p = System.Diagnostics.Process.Start("IExplore.exe", linkLabel1.Links[0].LinkData.ToString());
        }
    }
}

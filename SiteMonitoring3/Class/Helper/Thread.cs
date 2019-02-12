using libMyUtil;

namespace SiteMonitoring3.Helper
{
    public class Thread
    {
        public void SetLabel(System.Windows.Forms.Label label, string text)
        {
            clsThread.SetLabel(label, text);
        }

        public void buttonToggle(System.Windows.Forms.Button button, string text, bool enable)
        {
            clsThread.buttonToggle(button, text, enable);
        }

        public void SetTextBox(System.Windows.Forms.TextBox textBox, string text, string appendTextYn)
        {
            clsThread.SetTextBox(textBox, text, appendTextYn == "Y");
        }

        public delegate void SetTextBoxCallBack(System.Windows.Forms.TextBox textBox, string str, bool enable);
        public void SetTextBox(System.Windows.Forms.TextBox textBox, string str, bool enable)
        {
            if (textBox.InvokeRequired)
            {
                SetTextBoxCallBack dele = new SetTextBoxCallBack(SetTextBox);
                textBox.Invoke(dele, textBox, str, enable);
            }
            else
            {
                textBox.Enabled = enable;
                if (str.Length > 0)
                    textBox.AppendText(str);
            }
        }
        public delegate void SetCheckBoxCallBack(System.Windows.Forms.CheckBox chkBox, bool enable);
        public void SetCheckBox(System.Windows.Forms.CheckBox chkBox, bool enable)
        {
            if (chkBox.InvokeRequired)
            {
                SetCheckBoxCallBack dele = new SetCheckBoxCallBack(SetCheckBox);
                chkBox.Invoke(dele, chkBox, enable);
            }
            else
            {
                chkBox.Enabled = enable;
            }
        }
    }
}

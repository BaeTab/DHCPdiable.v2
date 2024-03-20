using System;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Win32;

namespace DHCPdisable.v2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            eventHandler();
            currentStatus();
        }

        private void eventHandler()
        {
            button3.Click += Button3_Click; // 재부팅
        }

        private void currentStatus()
        {
            try
            {
                // 레지스트리 키가 없을 경우 CreateSubKey 를 사용해야한다
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters"))
                {
                    var value = key.GetValue("IPAutoconfigurationEnabled");
                    // IPAutoconfigurationEnabled 키값이 이미 있는경우
                    if (null != value)
                    {
                        // IPAutoconfigurationEnabled 키값이 0인경우는 이미 비활성화 이다
                        if (0 == (int)value)
                        {
                            textBox1.AppendText("*APIPA 설정이 비활성화 되어 있습니다\r\n");
                        }
                        // 키값의 설정이 다른경우 설정해준다
                        else
                        {
                            key.SetValue("IPAutoconfigurationEnabled", 0, RegistryValueKind.DWord);
                            textBox1.AppendText("*APIPA 설정을 비활성화 했습니다\r\n");
                        }
                    }
                    // IPAutoconfigurationEnabled 키값이 없는 경우 0의 값으로 생성한다
                    else
                    {
                        key.SetValue("IPAutoconfigurationEnabled", 0, RegistryValueKind.DWord);
                        textBox1.AppendText("*APIPA 설정을 비활성화 했습니다\r\n");
                    }
                }
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Services\Dhcp"))
                {
                    var value = key.GetValue("Start");
                    if (null != value)
                    {
                        if (4 == (int)value)
                        {
                            textBox1.AppendText("*DHCP Client 서비스는 비활성화 되어 있습니다");
                        }
                        else
                        {
                            key.SetValue("Start", 4, RegistryValueKind.DWord);
                            textBox1.AppendText("*DHCP Client 서비스를 비활성화 했습니다\r\n*재부팅이 필요합니다");
                            button3.Enabled = true;
                        }
                    }
                    else
                    {
                        key.SetValue("Start", 4, RegistryValueKind.DWord);
                        textBox1.AppendText("*DHCP Client 서비스를 비활성화 했습니다\r\n*재부팅이 필요합니다");
                        button3.Enabled = true;
                    }
                }
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (DialogResult.Yes == MessageBox.Show("재부팅 하시겠습니까?\r\n'예(Y)'를 클릭하면 즉시 재부팅됩니다", "알림", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                {
                    Process.Start("shutdown.exe", "/r /t 0");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"오류: {ex.Message}");
            }
        }
    }
}

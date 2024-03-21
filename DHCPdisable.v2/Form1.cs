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
            button3.Click += Button3_Click; // 재부팅 버튼
        }

        private void currentStatus()        // 현재 상태 체크하고 변경이 필요한경우 키값 변경
        {
            try
            {
                // 레지스트리 키가 없을 경우 CreateSubKey 를 사용해서 새 키를 만들어줘야 한다
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters"))
                {
                    // IPAutoconfigurationEnabled 키가 없는 경우 null 이 나오고 있을경우 GetValue 를 통해 값이 몇인지 나온다
                    var value = key.GetValue("IPAutoconfigurationEnabled");     
                    // value 가 null 이 아니면 키가 존재한다
                    if (null != value) 
                    {
                        // IPAutoconfigurationEnabled 키값이 0인경우는 이미 비활성화 이다
                        if (0 == (int)value)
                        {
                            textBox1.AppendText("*APIPA 설정이 비활성화 되어 있습니다\r\n");
                        }
                        // 키값의 설정이 다른경우 0으로 설정해준다
                        else
                        {
                            key.SetValue("IPAutoconfigurationEnabled", 0, RegistryValueKind.DWord);
                            textBox1.AppendText("*APIPA 설정을 비활성화 했습니다\r\n");
                            button3.Enabled = true;
                            label1.Visible = true;
                            label1.Text = "재부팅 필요";
                        }
                    }
                    // IPAutoconfigurationEnabled 키값이 없는 경우 0의 값으로 생성한다
                    else
                    {
                        key.SetValue("IPAutoconfigurationEnabled", 0, RegistryValueKind.DWord);
                        textBox1.AppendText("*APIPA 설정을 비활성화 했습니다\r\n");
                        button3.Enabled = true;
                        label1.Visible = true;
                        label1.Text = "재부팅 필요";
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
                            // APIPA 비활성화로 버튼3이 활성화 되었으면 딱히 다시 활성화 할 필요가 없어 조건문 추가
                            if (button3.Enabled == false)
                            {
                                button3.Enabled = true;
                                label1.Visible = true;
                                label1.Text = "재부팅 필요";
                            }
                        }
                    }
                    else
                    {
                        key.SetValue("Start", 4, RegistryValueKind.DWord);
                        textBox1.AppendText("*DHCP Client 서비스를 비활성화 했습니다\r\n*재부팅이 필요합니다");
                        // APIPA 비활성화로 버튼3이 활성화 되었으면 딱히 다시 활성화 할 필요가 없어 조건문 추가
                        if (button3.Enabled == false)
                        {
                            button3.Enabled = true;
                            label1.Visible = true;
                            label1.Text = "재부팅 필요";
                        }
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
                MessageBox.Show($"오류: {ex.Message}","오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

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
            button1.Click += Button1_Click; // 실행 버튼
            button2.Click += Button2_Click; // 기본값 복구
            button3.Click += Button3_Click; // 재부팅 버튼
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("작업을 진행 하시겠습니까?", "알림", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
            {
                DisableSetting("IPAutoconfigurationEnabled", @"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters", 0);
                DisableSetting("Start", @"SYSTEM\CurrentControlSet\Services\Dhcp", 4);
            }
            currentStatus();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("기본값으로 복구합니다. 진행하시겠습니까?", "알림", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
            {
                EnableSetting("IPAutoconfigurationEnabled", @"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters", 1);
                EnableSetting("Start", @"SYSTEM\CurrentControlSet\Services\Dhcp", 2);
            }
            currentStatus();
        }

        private void DisableSetting(string valueName, string registryPath, int value)
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(registryPath))
                {
                    var regValue = key.GetValue(valueName);
                    if (regValue == null || (int)regValue != value)
                    {
                        key.SetValue(valueName, value, RegistryValueKind.DWord);
                        button3.Enabled = true;
                    }
                    else
                    {
                        MessageBox.Show($"{valueName} 설정이 이미 비활성화 되어 있습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"오류 : {ex.Message}");
            }
        }

        private void EnableSetting(string valueName, string registryPath, int value)
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(registryPath))
                {
                    var regValue = key.GetValue(valueName);
                    if (regValue == null || (int)regValue != value)
                    {
                        key.SetValue(valueName, value, RegistryValueKind.DWord);
                        button3.Enabled = true;
                    }
                    else
                    {
                        MessageBox.Show($"{valueName} 설정은 이미 활성화 되어 있습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"오류 : {ex.Message}");
            }
        }

        private void currentStatus()
        {
            textBox1.Clear();
            CheckSettingStatus("IPAutoconfigurationEnabled", @"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters", "*APIPA 설정", button2, button1, 0);
            CheckSettingStatus("Start", @"SYSTEM\CurrentControlSet\Services\Dhcp", "*DHCP Client 서비스", button2, button1, 4);

            if (button3.Enabled)
            {
                label1.Text = "재부팅 필요";
                MessageBox.Show("설정이 변경되어 재부팅이 필요합니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void CheckSettingStatus(string valueName, string registryPath, string statusMessage, Button enableButton, Button disableButton, int disabledValue)
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryPath))
                {
                    var value = key.GetValue(valueName);
                    if (value != null)
                    {
                        if ((int)value == disabledValue)
                        {
                            textBox1.AppendText($"{statusMessage}는 비활성화 되어 있습니다\r\n");
                            enableButton.Enabled = true;
                            disableButton.Enabled = false;
                        }
                        else
                        {
                            textBox1.AppendText($"{statusMessage}는 활성화 되어 있습니다\r\n");
                            enableButton.Enabled = false;
                            disableButton.Enabled = true;
                        }
                    }
                    else
                    {
                        textBox1.AppendText($"{statusMessage} 설정값이 없습니다. 실행버튼을 눌러 작업을 진행해 주세요\r\n");
                        enableButton.Enabled = true;
                    }
                }
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
                if (DialogResult.Yes == MessageBox.Show("재부팅 하시겠습니까?\r\n'예(Y)'를 클릭하면 즉시 재부팅됩니다 ", "알림", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                {
                    Process.Start("shutdown.exe", "/r /t 0");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

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

        // APIPA  ,DHCP 비활성화 버튼
        private void Button2_Click(object sender , EventArgs e)
        {
            if(DialogResult.Yes == MessageBox.Show("작업을 진행 하시겠습니까?", "알림", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
            {
                try
                {
                    // 레지스트리 키가 없을 경우 CreateSubKey 를 사용해서 새 키를 만들어줘야 한다
                    using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters"))
                    {
                        var value = key.GetValue("IPAutoconfigurationEnabled");
                        if (null != value)
                        {
                            // 0 비활성화 , 1 활성화
                            if (0 != (int)value)
                            {
                                key.SetValue("IPAutoconfigurationEnabled", 0, RegistryValueKind.DWord);
                                button3.Enabled = true;
                            }
                            else
                            {
                                MessageBox.Show("APIPA 설정이 이미 비활성화 되어 있습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        // IPAutoconfigurationEnabled 키값이 없는 경우 0의 값으로 키를 생성한다
                        else
                        {
                            key.SetValue("IPAutoconfigurationEnabled", 0, RegistryValueKind.DWord);
                            button3.Enabled = true;
                        }
                    }
                    using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Services\Dhcp"))
                    {
                        var value = key.GetValue("Start");
                        if (null != value)
                        {
                            if (4 != (int)value)
                            {
                                // 2가 기본값 , 4가 사용안함
                                key.SetValue("Start", 4, RegistryValueKind.DWord);
                                // APIPA 비활성화로 버튼3이 활성화 되었으면 딱히 다시 활성화 할 필요가 없어 조건문 추가
                                if (button3.Enabled == false)
                                {
                                    button3.Enabled = true;
                                }
                            }
                            else
                            {
                                MessageBox.Show("DHCP Client 서비스 설정이 이미 비활성화 되어 있습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        else
                        {
                            key.SetValue("Start", 4, RegistryValueKind.DWord);
                            if (button3.Enabled == false)
                            {
                                button3.Enabled = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 : {ex.Message}");
                }
            }
            currentStatus();
        }

        // 활성상태로 되돌리는 버튼
        private void Button1_Click(object sender , EventArgs e)
        {
            // 레지스트리 키가 없을 경우 CreateSubKey 를 사용해서 새 키를 만들어줘야 한다
            if (DialogResult.Yes == MessageBox.Show("기본값으로 복구합니다. 진행하시겠습니까?", "알림", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
            {
                try
                {
                    using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters"))
                    {
                        var value = key.GetValue("IPAutoconfigurationEnabled");
                        if (null != value)
                        {
                            // 0 비활성화 , 1 활성화
                            if (1 != (int)value)
                            {
                                key.SetValue("IPAutoconfigurationEnabled", 1, RegistryValueKind.DWord);
                                button3.Enabled = true;
                            }
                            else
                            {
                                MessageBox.Show("APIPA 설정이 이미 활성화 되어 있습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        // IPAutoconfigurationEnabled 키값이 없는 경우 1의 값으로 키를 생성한다
                        else
                        {
                            key.SetValue("IPAutoconfigurationEnabled", 1, RegistryValueKind.DWord);
                            button3.Enabled = true;
                        }
                    }
                    using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Services\Dhcp"))
                    {
                        var value = key.GetValue("Start");
                        if (null != value)
                        {
                            if (2 != (int)value)
                            {
                                // 2가 기본값 (자동) , 4가 사용안함
                                key.SetValue("Start", 2, RegistryValueKind.DWord);
                                if (button3.Enabled == false)
                                {
                                    button3.Enabled = true;
                                }
                            }
                            else
                            {
                                MessageBox.Show("DHCP Client 서비스 설정은 이미 활성화 되어 있습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        else
                        {
                            key.SetValue("Start", 2, RegistryValueKind.DWord);
                            if (button3.Enabled == false)
                            {
                                button3.Enabled = true;
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"오류 : {ex.Message}");
                }
            }
            currentStatus();
        }

        private void currentStatus()        // 현재 상태 체크
        {
            textBox1.Clear();
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters"))
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
                            button1.Enabled = true;
                            button2.Enabled = false;
                        }
                        else
                        {
                            textBox1.AppendText("*APIPA 설정이 활성화 되어 있습니다\r\n");
                            button1.Enabled = false;
                            button2.Enabled = true;
                        }
                    }
                    else
                    {
                        textBox1.AppendText("*APIPA 설정값이 없습니다 실행버튼을 눌러 작업을 진행해 주세요\r\n");
                        button1.Enabled = true;
                    }
                }
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\Dhcp"))
                {
                    var value = key.GetValue("Start");
                    if (null != value)
                    {
                        if (4 == (int)value)
                        {
                            textBox1.AppendText("*DHCP Client 서비스는 비활성화 되어 있습니다");
                            button1.Enabled = true;
                            button2.Enabled = false;
                        }
                        else
                        {
                            textBox1.AppendText("*DHCP Client 서비스는 활성화 되어 있습니다\r\n");
                            button1.Enabled = false;
                            button2.Enabled = true;
                        }
                    }
                    else
                    {
                        textBox1.AppendText("*DHCP Client 서비스 설정값이 없습니다 실행버튼을 눌러 작업을 진행해 주세요\r\n");
                        button1.Enabled = true;
                    }
                }
                if(button3.Enabled == true)
                {
                    label1.Text = "재부팅 필요";
                    MessageBox.Show("설정이 변경되어 재부팅이 필요합니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 재부팅 버튼
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
                MessageBox.Show($"오류: {ex.Message}","오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

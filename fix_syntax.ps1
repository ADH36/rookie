# Fix syntax errors in MainForm.Designer.cs
$content = Get-Content 'E:\rookie\rookie\MainForm.Designer.cs'

# Fix line 152 (index 151)
$content[151] = '            this.m_combo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(55))));'

# Fix line 290 (index 289)
$content[289] = '            this.gamesListView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38))));'

# Fix line 736 (index 735)
$content[735] = '            this.uninstallAppButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38))));'

# Fix line 757 (index 756)
$content[756] = '            this.pullAppToDesktopBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38))));'

# Fix line 820 (index 819)
$content[819] = '            this.settingsButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48))));'

# Write the fixed content back to the file
$content | Out-File 'E:\rookie\rookie\MainForm.Designer.cs' -Encoding UTF8

Write-Host "Fixed syntax errors in MainForm.Designer.cs"
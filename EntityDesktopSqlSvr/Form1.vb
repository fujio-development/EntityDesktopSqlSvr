Imports System.Text.RegularExpressions

Public Class Form1
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load

        Call FormDesignSetting()

    End Sub

    ''' <summary>商品を全件表示します。</summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ButtonQuery_Click(sender As Object, e As EventArgs) Handles ButtonQuery.Click

        'AppConfigを使わずDbConnectionを使うやり方
        'Dim connection = Common.DbProviderFactories.GetFactory("System.Data.SqlClient").CreateConnection()
        'connection.ConnectionString = "Server=(local)\SQLEXPRESS;Initial Catalog=EntitySample;Integrated Security=True"

        ''AppConfigを使わずSystem.Data.Entity.DatabaseクラスのDefaultConnectionFactoryプロパティに登録するやり方
        'Dim sb As New SqlClient.SqlConnectionStringBuilder()
        'Dim factory As New System.Data.Entity.Infrastructure.SqlConnectionFactory(sb.ConnectionString)
        'System.Data.Entity.Database.DefaultConnectionFactory = factory

        Using context As New ShohinContext()
            If context.ShohinData.Any() = False Then
                ' すでにデータが存在するなら追加しない。
                Call InitialData(context)
            End If

            If TextBoxShohinNum.Text = "" Then
                'For Each data As SyohinEntity In context.SyohinData 'DataReaderみたいな物？　データベースが存在しなければ初回ここで時間が掛かる
                '    RichTextBox1.AppendText(data.SyohinCode & data.SyohinName & data.EditDate & data.EditTime & data.Note & vbCrLf)
                'Next
                DataGridView1.DataSource = context.ShohinData.ToList()
                RichTextBox1.AppendText("全件表示しました。" & vbCrLf)
            Else
                'For Each data As SyohinEntity In context.SyohinData.Where(Function(x) x.SyohinCode = TextBoxSyohinCode.Text).ToArray()
                '    RichTextBox1.AppendText(data.SyohinCode & data.SyohinName & data.EditDate & data.EditTime & data.Note & vbCrLf)
                'Next
                DataGridView1.DataSource = context.ShohinData.Where(Function(x) x.ShohinCode = TextBoxShohinNum.Text).ToList()
                RichTextBox1.AppendText("商品コードの条件による抽出を行いました。" & vbCrLf)
            End If

            Call DataGridSetting()
        End Using

        Call TextBoxClear()

    End Sub

    ''' <summary>テキストボックスによる内容で商品を追加します。</summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ButtonInsert_Click(sender As Object, e As EventArgs) Handles ButtonInsert.Click

        If Regex.IsMatch(TextBoxShohinNum.Text, "^[0-9]{1,4}$") = False Then
            MessageBox.Show("商品番号は半角数値の0～9999でなければなりません。", "メッセージ", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Using context As New ShohinContext()
            context.ShohinData.Add(New ShohinEntity With {
                    .ShohinCode = TextBoxShohinNum.Text,
                    .ShohinName = TextBoxShohinName.Text,
                    .EditDate = Format(Now, "yyyyMMdd"),
                    .EditTime = Format(Now, "HHmmss"),
                    .Note = TextBoxNote.Text})
            context.SaveChanges()
        End Using
        RichTextBox1.AppendText("商品を追加しました。" & vbCrLf)

    End Sub

    ''' <summary>商品ID(NumId)による商品の更新を行います。</summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ButtonUpdate_Click(sender As Object, e As EventArgs) Handles ButtonUpdate.Click

        If DataGridView1.Rows.Count <= 0 Or LabelNumId.Text = "" Then
            MessageBox.Show("削除する商品行が選択がされていません", "商品IDなし", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        If Regex.IsMatch(TextBoxShohinNum.Text, "^[0-9]{1,4}$") = False Then
            MessageBox.Show("商品番号は半角数値の0～9999でなければなりません。", "メッセージ", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Using context As New ShohinContext()
            Dim data = context.ShohinData.Single(Function(x) x.NumId = LabelNumId.Text)
            data.ShohinCode = TextBoxShohinNum.Text
            data.ShohinName = TextBoxShohinName.Text
            data.EditDate = Format(Now, "yyyyMMdd")
            data.EditTime = Format(Now, "HHmmss")
            data.Note = TextBoxNote.Text
            context.SaveChanges()
        End Using
        RichTextBox1.AppendText("商品ID：" & LabelNumId.Text & "のレコードを更新しました。" & vbCrLf)

    End Sub

    ''' <summary>商品ID(NumId)による商品を削除します。</summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ButtonDelete_Click(sender As Object, e As EventArgs) Handles ButtonDelete.Click

        If DataGridView1.Rows.Count <= 0 Or LabelNumId.Text = "" Then
            MessageBox.Show("削除する商品行が選択がされていません", "商品IDなし", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        If Regex.IsMatch(TextBoxShohinNum.Text, "^[0-9]{1,4}$") = False Then
            MessageBox.Show("商品番号は半角数値の0～9999でなければなりません。", "メッセージ", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Using context As New ShohinContext()
            Dim data = context.ShohinData.Single(Function(x) x.NumId = LabelNumId.Text)
            context.ShohinData.Remove(data)
            context.SaveChanges()
        End Using
        RichTextBox1.AppendText("商品ID：" & LabelNumId.Text & "のレコードを削除しました。" & vbCrLf)

    End Sub

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick

        LabelNumId.Text = Integer.Parse(DataGridView1.CurrentRow.Cells("NumId").Value)
        TextBoxShohinNum.Text = DataGridView1.CurrentRow.Cells("ShohinCode").Value
        TextBoxShohinName.Text = DataGridView1.CurrentRow.Cells("ShohinName").Value
        TextBoxNote.Text = DataGridView1.CurrentRow.Cells("Note").Value

    End Sub

    Private Sub InitialData(ByVal context As ShohinContext)

        context.ShohinData.Add(New ShohinEntity() With {
                .ShohinCode = 7500,
                .ShohinName = "ｾﾄｳﾁﾚﾓﾝ",
                .EditDate = 20190417,
                .EditTime = 203145,
                .Note = "瀬戸内レモンです"})
        context.ShohinData.Add(New ShohinEntity() With {
                .ShohinCode = 2840,
                .ShohinName = "ﾘﾝｺﾞｼﾞｭｰｽ",
                .EditDate = 20050923,
                .EditTime = 102532,
                .Note = "果汁100%の炭酸飲料です"})
        context.ShohinData.Add(New ShohinEntity With {
                .ShohinCode = 1580,
                .ShohinName = "ｶﾌｪｵﾚ",
                .EditDate = 20160716,
                .EditTime = 91103,
                .Note = "200ml増量です"})
        context.ShohinData.Add(New ShohinEntity With {
                .ShohinCode = 270,
                .ShohinName = "ｳﾒｵﾆｷﾞﾘ",
                .EditDate = 20080825,
                .EditTime = 141520,
                .Note = "none"})
        context.SaveChanges()

    End Sub

    Private Sub FormDesignSetting()

        Me.Text = "EntityFramework +デスクトップアプリ + SQL Server"
        Me.Location = New Point(500, 200)
        Me.Size = New Size(800, 600)
        Me.MaximizeBox = False
        Me.MinimizeBox = False

        Me.DataGridView1.Location = New Point(25, 25)
        Me.DataGridView1.Size = New Size(730, 200)

        Me.RichTextBox1.Location = New Point(25, 235)
        Me.RichTextBox1.Size = New Size(350, 100)
        Me.RichTextBox1.ReadOnly = True

        Me.ButtonQuery.Text = "抽出"
        Me.ButtonQuery.Location = New Point(50, 470)
        Me.ButtonQuery.Size = New Size(150, 50)
        Me.ButtonQuery.TabIndex = 3

        Me.ButtonInsert.Text = "追加"
        Me.ButtonInsert.Location = New Point(230, 470)
        Me.ButtonInsert.Size = New Size(150, 50)
        Me.ButtonInsert.TabIndex = 4

        Me.ButtonUpdate.Text = "更新"
        Me.ButtonUpdate.Location = New Point(410, 470)
        Me.ButtonUpdate.Size = New Size(150, 50)
        Me.ButtonUpdate.TabIndex = 5

        Me.ButtonDelete.Text = "削除"
        Me.ButtonDelete.Location = New Point(590, 470)
        Me.ButtonDelete.Size = New Size(150, 50)
        Me.ButtonDelete.TabIndex = 6

        Me.Label1.Location = New System.Drawing.Point(385, 250)
        Me.Label1.AutoSize = False
        Me.Label1.Size = New System.Drawing.Size(75, 25)
        Me.Label1.Text = "商品ID："

        Me.Label2.Location = New System.Drawing.Point(385, 300)
        Me.Label2.AutoSize = False
        Me.Label2.Size = New System.Drawing.Size(75, 25)
        Me.Label2.Text = "商品番号："

        Me.Label3.Location = New System.Drawing.Point(385, 350)
        Me.Label3.AutoSize = False
        Me.Label3.Size = New System.Drawing.Size(75, 25)
        Me.Label3.Text = "商品名："

        Me.Label4.Location = New System.Drawing.Point(385, 400)
        Me.Label4.AutoSize = False
        Me.Label4.Size = New System.Drawing.Size(60, 25)
        Me.Label4.Text = "備考："

        Me.LabelNumId.Location = New Point(690, 250)
        Me.LabelNumId.AutoSize = False
        Me.LabelNumId.Size = New Size(60, 20)
        Me.LabelNumId.Text = ""
        Me.LabelNumId.TextAlign = ContentAlignment.TopRight

        Me.TextBoxShohinNum.Location = New Point(600, 300)
        Me.TextBoxShohinNum.Size = New Size(150, 19)
        Me.TextBoxShohinNum.TabIndex = 0

        Me.TextBoxShohinName.Location = New Point(550, 350)
        Me.TextBoxShohinName.Size = New Size(200, 19)
        Me.TextBoxShohinName.TabIndex = 1

        Me.TextBoxNote.Location = New Point(450, 400)
        Me.TextBoxNote.Size = New Size(300, 19)
        Me.TextBoxNote.TabIndex = 2

    End Sub

    Private Sub DataGridSetting()

        DataGridView1.Columns("NumId").HeaderText = "商品ID"
        DataGridView1.Columns("ShohinCode").HeaderText = "商品番号"
        DataGridView1.Columns("ShohinName").HeaderText = "商品名"
        DataGridView1.Columns("EditDate").HeaderText = "編集日付"
        DataGridView1.Columns("EditTime").HeaderText = "編集時刻"
        DataGridView1.Columns("Note").HeaderText = "備考"
        DataGridView1.Columns("NumId").Width = 70
        DataGridView1.Columns("Note").Width = 250
        DataGridView1.Columns("EditDate").DefaultCellStyle.Format = "0000/00/00"
        DataGridView1.Columns("EditTime").DefaultCellStyle.Format = "00:00:00"
        DataGridView1.AllowUserToAddRows = False
        DataGridView1.RowHeadersVisible = False
        DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        DataGridView1.ReadOnly = True

    End Sub

    Private Sub TextBoxClear()

        TextBoxShohinNum.Text = ""
        TextBoxShohinName.Text = ""
        TextBoxNote.Text = ""

    End Sub

End Class
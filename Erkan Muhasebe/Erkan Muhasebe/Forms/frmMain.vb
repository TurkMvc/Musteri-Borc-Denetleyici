Public Class frmMain

    Private db As myDbOperations.OLEDB

#Region "Methods"
    Private Function ConvertSingle(ByVal s As String) As Single
        Try
            s = s.Replace(".", ",")

            ConvertSingle = CType(s, Single)

        Catch ex As Exception
            ConvertSingle = 0
        End Try

        Return (ConvertSingle)
    End Function

    Private Sub FillProducts()
        db = New myDbOperations.OLEDB(Application.StartupPath & "\" & "db.mdb", "tblProducts")

        Dim dt As DataTable = New DataTable
        dt = db.Select()

        Dim nodes As ArrayList = New ArrayList

        If dt.Rows.Count <> 0 Then
            'Database'den �r�nlerin listesi al�n�r..
            For i As Integer = 0 To dt.Rows.Count - 1
                Dim dr As DataRow = dt.Rows(i)

                'Database bilgileri olu�turulacak node'�n tag '�na eklenmesi i�in nesnesi olu�turulur.
                Dim objPro As objProduct = New objProduct(CInt(dr(0).ToString), dr(2).ToString, CInt(dr(1).ToString), "", CInt(dr(3).ToString), dr(4).ToString, CType(dr(5).ToString, Boolean), dr(6).ToString)

                'Database'den okunan �r�n�n treeNode'� olu�turulur.
                Dim nd As TreeNode = New TreeNode(objPro.ProductName, 1, 1)
                nd.Tag = objPro

                If objPro.ProductIsCategory = False Then
                    nd.ToolTipText = "�r�n Kodu : " & objPro.ProductID.ToString & " " & vbCrLf & _
                                     "�r�n Ad� : " & objPro.ProductName & " " & vbCrLf & _
                                     "�r�n Adeti : " & objPro.ProductUnit.ToString & " " & vbCrLf & _
                                     "�r�n Birim Fiyat� : " & objPro.ProductUnitPrice.ToString & " " & vbCrLf & _
                                     "�r�n Bilgisi : " & objPro.ProductDescription & " "
                Else
                    nd.ToolTipText = "Kategori Ismi : " & objPro.ProductName & " "
                End If

                nodes.Add(nd)

                ''E�er hi� node eklenmemi�se yukardaki For Each d�ng�s� i�lemeyecek ve ilk node Arraylist'e eklenemeyecektir.Bu nedenle bu if sorgusu sadece 1 kere �al���r ilk node'�n nodes array'�na eklenemsi i�in.
                'If nodes.Count = 0 Then nodes.Add(nd)
            Next

            'Olu�turulan Node �nceden eklenmi� bir node'�n (kategorinin) alt eleman� ise ona g�re eleman� oldu�u node a eklenmesi sa�lan�r.
            For i As Integer = nodes.Count - 1 To 1 Step -1
                Dim nd1 As TreeNode = nodes(i)
                Dim objPro1 As objProduct = CType(nd1.Tag, objProduct)

                If objPro1.ProductIsCategory = False Then
                    If objPro1.ProductUnit < 5 Then nd1.ForeColor = Color.Red
                End If

                For k As Integer = i - 1 To 0 Step -1
                    'nd1'e y�klenen node'dan �nceki node'larla kar��la�t�rma yap�lmak i�in de�i�kenler 
                    'olu�turulur..
                    Dim nd2 As TreeNode = nodes(k)
                    Dim objPro2 As objProduct = CType(nd2.Tag, objProduct)

                    'E�er herhangi bir node'�n ba�ka bir node'�n alt node'� oldugu e�le�irse o node alt�na
                    'eklenir..
                    If objPro1.ProductCategoryID = objPro2.ProductID Then
                        nd2.Nodes.Add(nd1)
                        Exit For
                    End If
                Next

                nodes.RemoveAt(i)
            Next

            Me.trvProducts.Nodes.Clear()

            'Olu�turulmu� nodes dallanmalar�n� TreeView'a ekleme.
            For Each node As TreeNode In nodes
                Me.trvProducts.Nodes.Add(node)
            Next
        End If

        If Not Me.trvProducts.Nodes.Count = 0 Then
            If Not Me.trvProducts.Nodes(0).Nodes.Count = 0 Then Me.trvProducts.Nodes(0).Expand()
        End If
    End Sub

    Private Sub FillCustomers()
        db = New myDbOperations.OLEDB(Application.StartupPath & "\" & "db.mdb", "tblCustomers")

        Dim dt As DataTable = New DataTable
        dt = db.Select()

        Me.trvCustomers.Nodes.Clear()

        Dim custRootNode As TreeNode = New TreeNode("T�m M��teriler", 0, 0)
        custRootNode.ToolTipText = "Kategori : " & custRootNode.Text & " "

        Me.trvCustomers.Nodes.Add(custRootNode)

        If dt.Rows.Count <> 0 Then

            'Database'den �r�nlerin listesi al�n�r..
            For Each dr As DataRow In dt.Rows

                'Database bilgileri olu�turulacak node'�n tag '�na eklenmesi i�in nesnesi olu�turulur.
                Dim objCust As objCustomer = New objCustomer(CInt(dr(0).ToString), dr(1).ToString, dr(2).ToString, dr(3).ToString, dr(4).ToString, dr(5).ToString)

                'Database'den okunan �r�n�n treeNode'� olu�turulur.
                Dim nd As TreeNode = New TreeNode(objCust.CustomerName & " " & objCust.CustomerSurname, 0, 0)
                nd.Tag = objCust

                nd.ToolTipText = "M��teri Kodu : " & objCust.CustomerID.ToString & " " & vbCrLf & _
                                 "M��teri Ad� : " & objCust.CustomerName & " " & vbCrLf & _
                                 "M��teri Soyad� : " & objCust.CustomerSurname & " " & vbCrLf & _
                                 "M��teri Tel No : " & objCust.CustomerPhone & " " & vbCrLf & _
                                 "M��teri Firma : " & objCust.CustomerCompany & " " & vbCrLf & _
                                 "M��teri Bilgisi : " & objCust.CustomerDescription & " "

                custRootNode.Nodes.Add(nd)

            Next
        End If

        If Not Me.trvCustomers.Nodes(0).Nodes.Count = 0 Then
            Me.trvCustomers.Nodes(0).Expand()
            Me.tsslCustomerCount.Text = "M��teri Say�s� : " & Me.trvCustomers.Nodes(0).Nodes.Count.ToString
        End If

    End Sub

    Private Sub FillEmployeeForCustomer(ByVal sender As Object, ByVal e As EventArgs)
        If Not Me.trvCustomers.SelectedNode Is Nothing Then
            Dim nd As TreeNode = Me.trvCustomers.SelectedNode
            Me.trvCustomers.SelectedNode = Me.trvCustomers.Nodes(0)
            Me.trvCustomers.SelectedNode = nd
        End If

        Me.FillProducts()

    End Sub

    Private Sub BorcMiktari()

        Dim usdPrice As Single = 0
        Dim ytlPrice As Single = 0

        For Each ctrl As Control In Me.pnlCustomerDetailsList.Controls
            If TypeOf ctrl Is usrControlCustomerEmployee Then
                Dim usrCtrl As usrControlCustomerEmployee = CType(ctrl, usrControlCustomerEmployee)
                Dim objEmp As objCustomerEmployee = CType(usrCtrl.Tag, objCustomerEmployee)

                If usrCtrl.lblComingPrice.ForeColor = Color.Red Then
                    Dim Borc As Single = Me.ConvertSingle(objEmp.EmployeeTotal) - Me.ConvertSingle(objEmp.EmployeeComingTotal)

                    If objEmp.EmployeeIsDolar = True Then
                        usdPrice += Borc
                    Else
                        ytlPrice += Borc
                    End If
                End If
            End If
        Next

        Me.tsslUSDPrice.Text = "Dolar Borcu : " & usdPrice.ToString & " $"
        Me.tsslYTLPrice.Text = "YTL Borcu : " & ytlPrice.ToString & " YTL"
    End Sub
#End Region

    Private Sub frmMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.FillProducts()
        Me.FillCustomers()
    End Sub

#Region "Product Islemleri"
    Private Sub btnProductAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnProductAdd.Click
        If Not Me.trvProducts.SelectedNode Is Nothing Then
            Dim nd As TreeNode = Me.trvProducts.SelectedNode
            Dim objPro As objProduct = CType(nd.Tag, objProduct)

            If objPro.ProductIsCategory = True Then
                Dim frm As frmAddProduct = New frmAddProduct
                frm.StartPosition = FormStartPosition.CenterScreen
                frm.subID = CInt(objPro.ProductID)
                frm.subName = objPro.ProductName
                frm.mode = "insert"

                AddHandler frm.frmAddProduct_Closing, AddressOf frmAddProductClosed

                frm.Show()
            Else
                MessageBox.Show("Se�ti�iniz bir �r�nd�r.. �r�n eklemek i�in bir kategori se�melisiniz. Se�ece�iniz kategori ismi de�i�tirilecektir..", "Bir Kategori Se�ili De�il !", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            End If
        Else
            MessageBox.Show("L�tfen �r�n eklemek i�in �r�nler listenizde bir kategoriyi se�iniz..", "Se�ili Kategori Yok !!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
        End If

    End Sub

    Private Sub frmAddProductClosed(ByVal sender As Object, ByVal e As EventArgs)
        Me.FillProducts()
    End Sub

    'Handles btnProductAddCategory.Click,cmnuCategoryAdd.Click
    Private Sub btnProductAddCategory_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnProductAddCategory.Click, cmnuCategoryAdd.Click
        If Not Me.trvProducts.SelectedNode Is Nothing Then
            Dim node As TreeNode = Me.trvProducts.SelectedNode
            Dim objPro As objProduct = CType(node.Tag, objProduct)

            If objPro.ProductIsCategory = True Then
                Dim catName As String = InputBox("Eklemek istedi�iniz kategorinin ismini yaz�n�z.")
                catName = catName.Replace(",", " - ")

                If catName.Trim.Length = 0 Then
                    MessageBox.Show("Kategori ismi bo� b�rak�lamaz..", "Kategori Ismi Girilmedi !", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    Exit Sub
                End If

                db = New myDbOperations.OLEDB(Application.StartupPath & "\" & "db.mdb", "tblProducts")

                Dim ColsVals As ArrayList = New ArrayList
                ColsVals.Add(New myDbOperations.Parameters("prodSubId", objPro.ProductID.ToString, myDbOperations.OLEVariableTypes.Number))
                ColsVals.Add(New myDbOperations.Parameters("prodName", catName, myDbOperations.OLEVariableTypes.Text))
                ColsVals.Add(New myDbOperations.Parameters("prodCount", "0", myDbOperations.OLEVariableTypes.Number))
                ColsVals.Add(New myDbOperations.Parameters("prodUnitPrice", "0", myDbOperations.OLEVariableTypes.Text))
                ColsVals.Add(New myDbOperations.Parameters("prodIsCategory", "True", myDbOperations.OLEVariableTypes.YesNo))
                ColsVals.Add(New myDbOperations.Parameters("prodDescription", "", myDbOperations.OLEVariableTypes.Text))

                If db.Insert(ColsVals) = 1 Then
                    MessageBox.Show("Kategori kaydedilmi�tir...", "��lem Tamamlanm��t�r.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1)

                    Me.FillProducts()

                Else
                    MessageBox.Show("Kategori kaydedilememi�tir...", "Hata Olu�tu !", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1)
                End If
            Else
                MessageBox.Show("Se�ti�iniz bir �r�nd�r.. Kategori eklemek i�in bir kategori se�melisiniz.Ekleyece�iniz kategori se�ti�iniz kategori alt�na eklenecektir.", "Bir kategori Se�ili De�il !", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            End If
        Else
            MessageBox.Show("L�tfen kategori eklemek i�in �r�nler listenizde bir kategoriyi se�iniz..", "Se�ili Kategori Yok !!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
        End If
    End Sub

    'Handles btnProductEdit.Click,cmnuProductEdit.Click
    Private Sub btnProductEdit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnProductEdit.Click, cmnuProductEdit.Click
        If Not Me.trvProducts.SelectedNode Is Nothing Then
            Dim nd As TreeNode = Me.trvProducts.SelectedNode
            Dim objPro As objProduct = CType(nd.Tag, objProduct)

            If objPro.ProductIsCategory = False Then
                Dim frm As frmAddProduct = New frmAddProduct
                frm.StartPosition = FormStartPosition.CenterScreen
                frm.subID = CInt(objPro.ProductCategoryID)
                frm.subName = objPro.ProductName
                frm.mode = "update"

                AddHandler frm.frmAddProduct_Closing, AddressOf frmAddProductClosed

                frm.LoadProductDetails(objPro)
                frm.Show()
            Else
                MessageBox.Show("Se�ti�iniz bir kategoridir.. �r�n d�zeltmek i�in bir �r�n se�melisiniz.", "Bir �r�n Se�ili De�il !", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            End If
        Else
            MessageBox.Show("L�tfen �r�n d�zeltmek i�in �r�nler listenizde bir �r�n� se�iniz..", "Se�ili �r�n Yok !!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
        End If

    End Sub

    'Handles btnProductEditCategory.Click,cmnuCategoryEdit.Click
    Private Sub btnProductEditCategory_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnProductEditCategory.Click, cmnuCategoryEdit.Click
        If Not Me.trvProducts.SelectedNode Is Nothing Then
            Dim node As TreeNode = Me.trvProducts.SelectedNode
            Dim objPro As objProduct = CType(node.Tag, objProduct)

            If objPro.ProductIsCategory = True Then
                Dim catName As String = InputBox("Se�ti�iniz kategorinin yeni ismini yaz�n�z..")
                catName = catName.Replace(",", " - ")

                If catName.Trim.Length = 0 Then
                    MessageBox.Show("Kategori ismi bo� b�rak�lamaz..", "Kategori Ismi Girilmedi !", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    Exit Sub
                End If

                db = New myDbOperations.OLEDB(Application.StartupPath & "\" & "db.mdb", "tblProducts")

                Dim ColsVals As ArrayList = New ArrayList
                ColsVals.Add(New myDbOperations.Parameters("prodSubId", objPro.ProductCategoryID.ToString, myDbOperations.OLEVariableTypes.Number))
                ColsVals.Add(New myDbOperations.Parameters("prodName", catName, myDbOperations.OLEVariableTypes.Text))
                ColsVals.Add(New myDbOperations.Parameters("prodCount", "0", myDbOperations.OLEVariableTypes.Number))
                ColsVals.Add(New myDbOperations.Parameters("prodUnitPrice", "0", myDbOperations.OLEVariableTypes.Text))
                ColsVals.Add(New myDbOperations.Parameters("prodIsCategory", "True", myDbOperations.OLEVariableTypes.YesNo))
                ColsVals.Add(New myDbOperations.Parameters("prodDescription", "", myDbOperations.OLEVariableTypes.Text))

                If db.Update(New myDbOperations.Parameters("prodId", objPro.ProductID.ToString, myDbOperations.OLEVariableTypes.Number), ColsVals) = 1 Then
                    MessageBox.Show("Kategori ismi de�i�tirilmi�tir...", "��lem Tamamlanm��t�r.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1)

                    Me.FillProducts()

                Else
                    MessageBox.Show("Kategori ismi de�i�tirilemedi...", "Hata Olu�tu !", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1)
                End If
            Else
                MessageBox.Show("Se�ti�iniz bir �r�nd�r.. Kategori ismi de�i�tirmek i�in bir kategori se�melisiniz. Se�ece�iniz kategori ismi de�i�tirilecektir..", "Bir kategori Se�ili De�il !", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            End If
        Else
            MessageBox.Show("L�tfen kategori ismi de�i�tirmek i�in �r�nler listenizde bir kategoriyi se�iniz..", "Se�ili Kategori Yok !!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
        End If

    End Sub

    'Handles btnProductDelete.Click,cmnuProductDel.Click
    Private Sub btnProductDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnProductDelete.Click, cmnuProductDel.Click
        If Not Me.trvProducts.SelectedNode Is Nothing Then
            Dim node As TreeNode = Me.trvProducts.SelectedNode
            Dim objPro As objProduct = CType(node.Tag, objProduct)

            If objPro.ProductIsCategory = False Then

                If MessageBox.Show(objPro.ProductName & " adl� �r�n� silmek istedi�inizden emin misiniz?", "�r�n Silinsin Mi?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) = Windows.Forms.DialogResult.Yes Then
                    db = New myDbOperations.OLEDB(Application.StartupPath & "\" & "db.mdb", "tblProducts")

                    If db.Delete(New myDbOperations.Parameters("prodId", objPro.ProductID.ToString, myDbOperations.OLEVariableTypes.Number)) = 1 Then
                        MessageBox.Show("�r�n silinmi�tir...", "��lem Tamamlanm��t�r.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1)

                        Me.FillProducts()

                    Else
                        MessageBox.Show("�r�n silinememi�tir...", "Hata Olu�tu !", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1)
                    End If
                End If
            Else
                MessageBox.Show("Se�ti�iniz bir kategoridir.. �r�n silmek i�in bir �r�n se�melisiniz..", "Bir �r�n Se�ili De�il !", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            End If
        Else
            MessageBox.Show("L�tfen �r�n silmek i�in �r�nler listenizde bir �r�n se�iniz..", "Se�ili �r�n Yok !!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
        End If
    End Sub

    'Handles btnProductDelCategory.Click,cmnuCategoryDel.Click
    Private Sub btnProductDelCategory_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnProductDelCategory.Click, cmnuCategoryDel.Click
        If Not Me.trvProducts.SelectedNode Is Nothing Then
            Dim node As TreeNode = Me.trvProducts.SelectedNode
            Dim objPro As objProduct = CType(node.Tag, objProduct)

            If objPro.ProductIsCategory = True Then

                If node.Nodes.Count <> 0 Then
                    If MessageBox.Show("Silmek istedi�iniz kategori �r�nler i�eriyor.. Bu kategoriyi silerseniz �r�nlerde silinecektir !" & vbCrLf & vbCrLf & "Devam etmek istiyor musunuz?", "Kategori Sil !", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1) = Windows.Forms.DialogResult.Yes Then
                        For Each childNode As TreeNode In node.Nodes
                            Dim childObjPro As objProduct = CType(childNode.Tag, objProduct)

                            db = New myDbOperations.OLEDB(Application.StartupPath & "\" & "db.mdb", "tblProducts")
                            db.Delete(New myDbOperations.Parameters("prodId", childObjPro.ProductID.ToString, myDbOperations.OLEVariableTypes.Number))

                        Next

                        GoTo KategoriSil
                    End If
                Else

                    If MessageBox.Show(objPro.ProductName & " isimli kategoriyi silmek istedi�inizden emin misiniz?", "Kategori Sil", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) = Windows.Forms.DialogResult.Yes Then

KategoriSil:
                        db = New myDbOperations.OLEDB(Application.StartupPath & "\" & "db.mdb", "tblProducts")

                        If db.Delete(New myDbOperations.Parameters("prodId", objPro.ProductID.ToString, myDbOperations.OLEVariableTypes.Number)) = 1 Then
                            MessageBox.Show("Kategori silinmi�tir...", "��lem Tamamlanm��t�r.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1)

                            Me.FillProducts()

                        Else
                            MessageBox.Show("Kategori silinememi�tir...", "Hata Olu�tu !", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1)
                        End If
                    End If

                End If

            Else
                MessageBox.Show("Se�ti�iniz bir �r�nd�r.. Kategori silmek i�in bir kategori se�melisiniz..", "Bir kategori Se�ili De�il !", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            End If
        Else
            MessageBox.Show("L�tfen kategori silmek i�in �r�nler listenizde bir kategoriyi se�iniz..", "Se�ili Kategori Yok !!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
        End If

    End Sub

    Private Sub trvProducts_AfterSelect(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles trvProducts.AfterSelect
        Dim nd As TreeNode = Me.trvProducts.SelectedNode
        Dim objPro As objProduct = CType(nd.Tag, objProduct)
        Dim info As String = "�r�n Ad� : "

        If objPro.ProductIsCategory = True Then info = "Kategori Ad� : "

        info &= objPro.ProductName

        Me.tsslProductCount.Text = info
    End Sub
#End Region

#Region "Customer Islemleri"
    Private Sub frmAddCustomerClosed(ByVal sender As Object, ByVal e As EventArgs)
        Me.FillCustomers()
    End Sub

    Private Sub btnCustomerAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCustomerAdd.Click

        Dim nd As TreeNode = Me.trvCustomers.Nodes(0)

        Dim frm As frmAddCustomer = New frmAddCustomer
        frm.StartPosition = FormStartPosition.CenterScreen
        frm.mode = "insert"

        AddHandler frm.frmAddcustomer_Closing, AddressOf frmAddCustomerClosed

        frm.Show()

    End Sub

    Private Sub btnCustomerEdit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCustomerEdit.Click
        If Not Me.trvCustomers.SelectedNode Is Nothing Then
            Dim nd As TreeNode = Me.trvCustomers.SelectedNode
            Dim objCust As objCustomer = CType(nd.Tag, objCustomer)

            Dim frm As frmAddCustomer = New frmAddCustomer
            frm.StartPosition = FormStartPosition.CenterScreen
            frm.mode = "update"

            AddHandler frm.frmAddcustomer_Closing, AddressOf Me.frmAddCustomerClosed

            frm.LoadcustomerDetails(objCust)
            frm.Show()
        Else
            MessageBox.Show("L�tfen m��teri d�zeltmek i�in m��teriler listenizde bir m��teriyi se�iniz..", "Se�ili M��teri Yok !!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
        End If

    End Sub

    Private Sub btnCustomerDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCustomerDelete.Click
        If Not Me.trvCustomers.SelectedNode Is Nothing Then
            Dim node As TreeNode = Me.trvCustomers.SelectedNode
            Dim objCust As objCustomer = CType(node.Tag, objCustomer)

            If MessageBox.Show(objCust.CustomerName & " " & objCust.CustomerSurname & " adl� m��teriyi silmek istedi�inizden emin misiniz?", "M��teri Silinsin Mi?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) = Windows.Forms.DialogResult.Yes Then
                db = New myDbOperations.OLEDB(Application.StartupPath & "\" & "db.mdb", "tblCustomers")

                If db.Delete(New myDbOperations.Parameters("custId", objCust.CustomerID.ToString, myDbOperations.OLEVariableTypes.Number)) = 1 Then

                    db = New myDbOperations.OLEDB(Application.StartupPath & "\" & "db.mdb", "tblCustomerEmployees")
                    db.Delete(New myDbOperations.Parameters("empCustId", objCust.CustomerID.ToString, myDbOperations.OLEVariableTypes.Number))

                    MessageBox.Show("M��teri silinmi�tir...", "��lem Tamamlanm��t�r.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1)

                    Me.FillCustomers()

                    Me.pnlCustomerDetailsList.Controls.Clear()

                Else
                    MessageBox.Show("M��teri silinememi�tir...", "Hata Olu�tu !", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1)
                End If
            End If
        Else
            MessageBox.Show("L�tfen m��teri silmek i�in M��teriler listenizde bir m��teriyi se�iniz..", "Se�ili M��teri Yok !!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
        End If
    End Sub

    Private Sub trvCustomers_AfterSelect(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles trvCustomers.AfterSelect
        Dim nd As TreeNode = Me.trvCustomers.SelectedNode

        If Not nd.Tag Is Nothing Then
            Me.pnlCustomerDetailsList.Controls.Clear()
            Dim objCust As objCustomer = CType(nd.Tag, objCustomer)

            db = New myDbOperations.OLEDB(Application.StartupPath & "\" & "db.mdb", "tblCustomerEmployees")

            Dim dt As DataTable = New DataTable
            dt = db.RunQuery(myDbOperations.QueryType.Select, "SELECT * FROM " & db.MyDbTable & " WHERE empCustId=" & objCust.CustomerID)

            If Not dt.Rows.Count = 0 Then
                For Each dr As DataRow In dt.Rows
                    Dim objEmp As objCustomerEmployee = New objCustomerEmployee(CInt(dr(0).ToString), CInt(dr(1).ToString), CInt(dr(2).ToString), dr(3).ToString, CDate(dr(4).ToString), CInt(dr(5).ToString), dr(6).ToString, dr(7).ToString, dr(8).ToString, CType(dr(9).ToString, Boolean), "")

                    Dim usrCtrl As usrControlCustomerEmployee = New usrControlCustomerEmployee
                    usrCtrl.Dock = DockStyle.Top

                    usrCtrl.FillDetails(objEmp)

                    Me.pnlCustomerDetailsList.Controls.Add(usrCtrl)
                Next
            End If

            Me.tsslCustDetailsEmpCount.Text = "M��terinin Toplam Sipari� Adedi : " & Me.pnlCustomerDetailsList.Controls.Count.ToString

            BorcMiktari()
        End If
    End Sub
#End Region

#Region "Employee Islemleri"
    Private Sub btnEmployeeAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEmployeeAdd.Click

        If Not Me.trvCustomers.SelectedNode.Tag Is Nothing Then
            If Not Me.trvProducts.SelectedNode Is Nothing Then

                Dim ndProd As TreeNode = Me.trvProducts.SelectedNode
                Dim objProd As objProduct = CType(ndProd.Tag, objProduct)

                If Not objProd.ProductIsCategory = True Then

                    Dim ndCust As TreeNode = Me.trvCustomers.SelectedNode
                    Dim objCust As objCustomer = CType(ndCust.Tag, objCustomer)

                    Dim frm As frmAddEmployee = New frmAddEmployee
                    AddHandler frm.frmAddEmployee_Closed, AddressOf Me.FillEmployeeForCustomer

                    frm.FillCustomers()
                    frm.FillProducts()
                    frm.SelectCustAndProd(objCust, objProd)

                    frm.Show()
                Else
                    MessageBox.Show("Sipari� 'i olu�turmak i�in bir kategori se�tiniz.. L�tfen �r�nler listenizden eklemek istedi�iniz �r�n� se�iniz..", "Se�ili �r�n Yok !", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                End If
            Else
                MessageBox.Show("Sipari� 'i olu�turmak i�in bir �r�n se�mediniz. L�tfen �r�nler listenizden eklemek istedi�iniz �r�n� se�iniz..", "Se�ili �r�n Yok !", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            End If
        Else
            MessageBox.Show("Sipari� 'i eklemek istedi�iniz m��teriyi se�mediniz.. L�tfen m��teriler listenizden bir m��teri se�iniz..", "Se�ili M��teri Yok !", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
        End If

    End Sub

    Private Sub btnEmployeeEdit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEmployeeEdit.Click
        For Each ctrl As Control In Me.pnlCustomerDetailsList.Controls
            If TypeOf ctrl Is usrControlCustomerEmployee Then
                Dim usrCtrl As usrControlCustomerEmployee = CType(ctrl, usrControlCustomerEmployee)

                If usrCtrl.chkEmployeeDate.Checked Then
                    Dim objemp As objCustomerEmployee = CType(usrCtrl.Tag, objCustomerEmployee)

                    Dim frm As frmAddEmployee = New frmAddEmployee
                    AddHandler frm.frmAddEmployee_Closed, AddressOf Me.FillEmployeeForCustomer

                    frm.FillCustomers()
                    frm.FillProducts()
                    frm.SelectCustAndProd(objemp)

                    frm.ShowDialog()

                End If

            End If
        Next
    End Sub

    Private Sub btnEmployeeDel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEmployeeDel.Click

        Dim IsDel As Boolean = False
        Dim Err As Boolean = False
        Dim IsDelEmployee As Boolean = False

        For Each ctrl As Control In Me.pnlCustomerDetailsList.Controls
            If TypeOf ctrl Is usrControlCustomerEmployee Then
                Dim usrCtrl As usrControlCustomerEmployee = CType(ctrl, usrControlCustomerEmployee)
                Dim objEmp As objCustomerEmployee = CType(usrCtrl.Tag, objCustomerEmployee)

                If IsDel = False Then
                    If usrCtrl.chkEmployeeDate.Checked Then
                        If MessageBox.Show("Se�ili sipari� yada sipari�ler silinsin mi?", "Sipari�(ler) Silinsin Mi?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) = Windows.Forms.DialogResult.Yes Then
                            IsDel = True
                        Else
                            Exit Sub
                        End If
                    End If
                End If

                If IsDel = True Then
                    If usrCtrl.chkEmployeeDate.Checked Then
                        Dim dbEmp As myDbOperations.OLEDB = New myDbOperations.OLEDB(Application.StartupPath & "\" & "db.mdb", "tblCustomerEmployees")

                        If dbEmp.Delete(New myDbOperations.Parameters("empId", objEmp.EmployeeID.ToString, myDbOperations.OLEVariableTypes.Number)) = 0 Then
                            Err = True
                            Exit For
                        Else
                            IsDelEmployee = True
                        End If
                    End If
                End If
            End If
        Next

        If Err = True Then
            MessageBox.Show("Baz� sipari� yada sipari�lerin silinmesi s�ras�nda hata olu�tu..", "Hata Olu�tu !", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1)
        End If

        If IsDelEmployee = True Then FillEmployeeForCustomer(sender, e)
    End Sub
#End Region

#Region "ConProducts"
    Private Sub conProduct_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles conProduct.Opening

        For Each itm As ToolStripItem In Me.conProduct.Items
            itm.Enabled = True
        Next

        If Not Me.trvProducts.SelectedNode Is Nothing Then
            Dim selNode As TreeNode = Me.trvProducts.SelectedNode
            Dim objProd As objProduct = CType(selNode.Tag, objProduct)

            If objProd.ProductIsCategory = False Then
                '�r�nler t�kland�ysa..
                Me.cmnuCategoryAdd.Enabled = False
                Me.cmnuCategoryEdit.Enabled = False
                Me.cmnuCategoryDel.Enabled = False

                db = New myDbOperations.OLEDB(Application.StartupPath & "\" & "db.mdb", "tblProducts")
                Dim dt As DataTable = New DataTable

                Me.cmnuProductMove.DropDownItems.Clear()

                dt = db.RunQuery(myDbOperations.QueryType.Select, "SELECT prodId,prodName FROM tblProducts WHERE prodIsCategory=True ORDER BY tblProducts.prodName")

                Dim newItem As ToolStripMenuItem

                For Each dr As DataRow In dt.Rows
                    newItem = New ToolStripMenuItem(dr(1).ToString, Me.imgList.Images(2), New System.EventHandler(AddressOf conProducts_Onclick))
                    newItem.Tag = CInt(dr(0).ToString)

                    Me.cmnuProductMove.DropDownItems.Add(newItem)
                Next
            Else
                'Kategori t�kland�ysa ..
                Me.cmnuProductDel.Enabled = False
                Me.cmnuProductEdit.Enabled = False
                Me.cmnuProductMove.Enabled = False
            End If
        End If
    End Sub

    Private Sub conProducts_Onclick(ByVal sender As Object, ByVal e As EventArgs)
        If Not Me.trvProducts.SelectedNode Is Nothing Then
            Dim objProd As objProduct = CType(Me.trvProducts.SelectedNode.Tag, objProduct)
            Dim catId As String = CType(sender, ToolStripMenuItem).Tag.ToString

            db = New myDbOperations.OLEDB(Application.StartupPath & "\" & "db.mdb", "tblProducts")
            db.Delete(New myDbOperations.Parameters("prodId", objProd.ProductID.ToString, myDbOperations.OLEVariableTypes.Number))

            Dim ColsVals As ArrayList = New ArrayList
            ColsVals.Add(New myDbOperations.Parameters("prodSubId", catId, myDbOperations.OLEVariableTypes.Number))
            ColsVals.Add(New myDbOperations.Parameters("prodName", objProd.ProductName, myDbOperations.OLEVariableTypes.Text))
            ColsVals.Add(New myDbOperations.Parameters("prodCount", objProd.ProductUnit.ToString.ToString, myDbOperations.OLEVariableTypes.Number))
            ColsVals.Add(New myDbOperations.Parameters("prodUnitPrice", objProd.ProductUnitPrice.ToString, myDbOperations.OLEVariableTypes.Text))
            ColsVals.Add(New myDbOperations.Parameters("prodIsCategory", "False", myDbOperations.OLEVariableTypes.YesNo))
            ColsVals.Add(New myDbOperations.Parameters("prodDescription", objProd.ProductDescription, myDbOperations.OLEVariableTypes.Text))

            db.Insert(ColsVals)

            Me.FillProducts()
        End If
    End Sub

    Private Sub cmnuProductDetails_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmnuProductDetails.Click
        If Not Me.trvProducts.SelectedNode Is Nothing Then
            Dim selNode As TreeNode = Me.trvProducts.SelectedNode
            Dim objPro As objProduct = CType(selNode.Tag, objProduct)
            Dim str As System.Text.StringBuilder = New System.Text.StringBuilder
            Dim strMain As String = ""

            If objPro.ProductIsCategory = False Then
                strMain = "�r�n Bilgisi"
                str.Append("�r�n Kodu : " & objPro.ProductID.ToString)
                str.AppendLine()
                str.Append("�r�n Ad� : " & objPro.ProductName)
                str.AppendLine()
                str.Append("�r�n Adedi : " & objPro.ProductUnit.ToString)
                str.AppendLine()
                str.Append("�r�n Birim Fiyat� : " & objPro.ProductUnitPrice)
                str.AppendLine()
                str.Append("�r�n A��klama : " & objPro.ProductDescription)
                str.AppendLine()
            Else
                strMain = "Kategori Bilgisi"
                str.Append("Kategori Kodu : " & objPro.ProductID.ToString)
                str.AppendLine()
                str.Append("Kategori Ad� : " & objPro.ProductName)
                str.AppendLine()
                str.Append("Kategori A��klama : " & objPro.ProductDescription)
                str.AppendLine()
            End If

            MessageBox.Show(str.ToString, strMain, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1)
        End If
    End Sub
#End Region

End Class

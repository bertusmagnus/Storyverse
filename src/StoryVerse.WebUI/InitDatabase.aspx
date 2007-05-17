<%@ Page Language="C#" Debug="true" %>
<%@ Import namespace="Castle.ActiveRecord"%>
<%@ Import namespace="StoryVerse.Core.Models"%>
<script runat="server">
    
    protected override void OnLoad(EventArgs e)
    {
        lblConString.Text = ActiveRecordMediator.GetSessionFactoryHolder()
            .GetAllConfigurations()[0]
            .Properties["hibernate.connection.connection_string"].ToString(); ;
    }

    protected void btnInitDatabase_Click(object sender, EventArgs e)
    {
        try
        {
            ActiveRecordStarter.CreateSchema();
            lblCreateDbResult.Text = "<br>- Database was initialized";
            lblCreateDbResult.ForeColor = System.Drawing.Color.Green;
            try
            {
                Company c = new Company();
                c.Name = "getin";
                Person p = new Person();
                p.FirstName = "Get";
                p.LastName = "In";
                p.Username = "getin";
                p.Password = "letmein";
                p.IsAdmin = true;
                c.AddEmployee(p);
                c.CreateAndFlush();
                lblCreateCompanyResult.Text = string.Format("<br>- Test user created: Username={0} Password={1}", p.Username, p.Password);
                lblCreateCompanyResult.ForeColor = System.Drawing.Color.Green;
                lbGoToLogin.Visible = true;
            }
            catch (Exception ex)
            {
                lblCreateCompanyResult.Text = "<br>- Test user NOT created...<br><br>" + ex.ToString();
                lblCreateCompanyResult.ForeColor = System.Drawing.Color.Red;
            }
        }
        catch (Exception ex)
        {
            lblCreateDbResult.Text = "<br>- Database was NOT initialized...<br><br>" + ex.ToString();
            lblCreateDbResult.ForeColor = System.Drawing.Color.Red;
        }
    }

    protected void lbGoToLogin_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/projects/list.rails");
    }
    
</script>

<!DOCTYPE html PUBLIC 
  "-//W3C//DTD XHTML 1.0 Strict//EN"
  "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
  <title>Initialize  StoryVerse Database</title>
</head>
<body style="font-family:Arial,Hevetica;">
    <form runat="server">
        <h1>Initialize StoryVerse Database</h1>
        <table style="width:600px;">
            <tr>
                <td style="white-space:nowrap; vertical-align:top;">Step 1:</td>
                <td style="padding-left:10px;">Edit the connection string in the "hibernate.connection.connection_string" key in the Web.config file for the WebUI project.  The currect connection string is:</td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td style="padding:10px 0px 10px 10px;">
                    <asp:Label ID="lblConString" runat="server" Text="" ForeColor="blue" Font-Size="smaller"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="white-space:nowrap">Step 2:</td>
                <td style="padding-left:10px;">Create a blank database based on the connection string</td>
            </tr>
            <tr>
                <td style="white-space:nowrap">Step 3:</td>
                <td style="padding-left:10px;">Click the the "Initialize Database" button...</td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td style="padding:30px 0px 30px 10px;">
                    <asp:Button ID="btnInitDatabase" runat="server" Text="Initialize Database" OnClick="btnInitDatabase_Click" style="height:50px; width:250px; font-size: 20px;" /><br />
                    <asp:Label ID="lblCreateDbResult" runat="server" Text="" ForeColor="green"></asp:Label>
                    <asp:Label ID="lblCreateCompanyResult" runat="server" Text="" ForeColor="green"></asp:Label><br />
                    <asp:LinkButton ID="lbGoToLogin" Visible="false" runat="server" OnClick="lbGoToLogin_Click"><br />Proceed To Login</asp:LinkButton></td>
            </tr>
            <tr>
                <td style="color:Red;" colspan="2">
                WARNING: Clicking "Initialize Database" will overwrite existing data tables in the specified database.  This could result in PERMANENT DATA LOSS!!!  Proceed with caution.  To reduce the risk of data loss, you might consider removing this page from the website once you have successfully initialized your database.</td>
            </tr>
        </table>
    </form>
</body>
</html>
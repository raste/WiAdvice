### Preparations

In order to modify the code and build the application you will need Visual Studio 2012 or greater.

To run the site you will have to prepare couple of things:  

1. Databases 
  Microsoft SQL Server 2008 or higher is needed to host the databases.  
  You will need to create 3 databases, one for users, one for english version and one for the bulgarian version. You can do that by running the scripts or restoring via the backup files in [/DB folder](https://github.com/raste/WiAdvice/tree/master/DB). 

   *NOTE: There are stored procedures (in the languages databases), which use the user database. If you name it other than 'wiadvice_userDB' make sure to update the procedures (search in the script files for 'wiadvice_userDB')*

2. Connection strings configuration  
  The connections to the databases must be configured in [Web.config file](https://github.com/raste/WiAdvice/blob/master/Source/User%20Interface/Web.config).  

  ```
  <connectionStrings>
    <add name="Entities_bg" ....  />
    <add name="Entities_en" .... />
    <add name="EntitiesUsers" .... />
  </connectionStrings>
  ``` 
  are the lines used by the application to connect with the databases. If they are not set up correctly, the site will show error on start up.

  The sections, which need to be modified (analogous for the three connection strings):
    * `Data Source=NAME;` - replace `NAME` with the name and address (if it is located on other machine) of the SQL server 
    * `Initial Catalog=wiadvice_userDB;` - replace `wiadvice_userDB` with the name of the created database for users (analogous for others)
    *  If the databases are password protected remove `Integrated Security=True;` and add `;uid=username;pwd=password` after `MultipleActiveResultSets=True` and substitute `username` and `password` with your credentials.
      The connections strings will look like this whit credentials  
  ```
<add name="Entities_bg" connectionString="metadata=res://*/DatabaseModel.csdl|res://*/DatabaseModel.ssdl|res://*/DatabaseModel.msl;provider=System.Data.SqlClient;provider connection string=&quot;Data Source=NAME;Initial Catalog=wiadvice_siteDB_BG;MultipleActiveResultSets=True;uid=username;pwd=password&quot;" providerName="System.Data.EntityClient" />
 ```  
   
   **IMPORTANT**: Each connection string line must be on ONE row.
3. Logs configuration (again in web.config file)  
  This project uses open source library "log4net" [logging.apache.org/](http://logging.apache.org/) for logging of exceptions (errors) to log files. Basically if site crashes at some operation, the error will be written to a log file.
  
  You may not be interested in this functionality, so there are 2 options:
    * To not use logging: 
      replace `<level value="DEBUG" />` with `<level value="OFF" />` in *configuration > log4net > root* node or replace `D:\Logs\WiAdvice.log` in `<param name="File" value="D:\Logs\WiAdvice.log" />` with invalid path.
    * To use logging:
      Create directory in which the logs files will be created.  
      
      *NOTES when application is uploaded to server:*  
         * The logs directory must be sub-directory of the application dir.  
         * You need to explicitly give rights to the Worker process (Example name : Plesk IIS WP User (ASPNET_WP), name is different based on provider), in order application to read/write log files. The necessary permissions are : READ, WRITE, MODIFY, READ AND EXECUTE. 
      
      Replace `D:\Logs\WiAdvice.log` in `<param name="File" value="D:\Logs\WiAdvice.log" />` with the physical path of the logs directory. An log file will be created on first start up if everything is done correctly.
      
      *NOTE: Be sure that the log files cannot be downloaded by clients by typing the address of a log file in a browser.*
4. Emails configuration (again web.config)  
   Replace `mail@domain.com` in `<add key="SiteGeneralSectionMail" value="mail@domain.com" />` with the address who should receive general questions emails (from About form)  
   Same in `<add key="SiteSupportSectionMail" value="mail@domain.com" />` for the address who should receive support emails (from About form)  
   Repeat for `<add key="SiteAdvertisementsSectionMail" value="mail@domain.com" />` for advertisement emails  
   
   in
  ```
  <mailSettings>
      <smtp deliveryMethod="Network" from="mail@domain.com">
        <network host="mail.domain.com" defaultCredentials="false" userName="mail@domain.com" password="mailPass" />
      </smtp>
  </mailSettings>
  ```  
  Substitute `mail.domain.com` with the mail subdomain from which emails will be sent. Replace `mail@domain.com` in both places with the email address, from which all emails will be sent. Substitute `mailPass` in `password="mailPass"` with the password of the chosen email address.  
  
  *NOTE: SMTP must be enabled for the email address, in order to send emails from it.*  
5. Set [Home.aspx](https://github.com/raste/WiAdvice/blob/master/Source/User%20Interface/Home.aspx) as start page in Visual Studio. 
5. Development server or IIS  
   The web.config file is configured to allow running the application from Visual Studio Development Server. There is one limitation though, it can not use URL Rewrite > the language change links will not work.  
   
   In order to use URL Rewriting you will have to run the project from the Local IIS Web server, which must have URL Rewrite plugin. You will have to update these configurations in web.config (find the settings and replace their values)  

   ```
<add key="UseUrlRewriting" value="true" />
<add key="UrlRewritingDirectoryLevel" value="1" />
<add key="UseExternalUrlRewriteModule" value="true" />
<add key="SiteDomainAdress" value="TYPE IIS SITE ADDRESS " /> 

<add name="RewriteUrlModule" type="UserInterface.RewriteUrl" />
   ```  
   
6. Other configurations  
   Varius aspects of the site can be configured with the other settings in `<appSettings>` after `<!-- SITE CONFIGURATION  -->` comment. You will have to figure them by their names.

### Run

Now you are ready to run the application.  

Initial users:  
  * The main administrator is with username `admin` and password `admin`. To change the password log in with him and go to profile page.

**If there are problems**:  
  * If you specified valid directory for "log4net", and set everything correct, there should be a log file in the specified directory. Open it and see if there are ERROR entries.  
  * If you haven't configured "log4net" or it isn't writing logs, you can change the following line in Web.config: `<customErrors mode="On" />` with `<customErrors mode="Off" />`. This will enable the application to show the errors in the browser (but this way everyone will be able to see them if they encounter one). After this start the project and see what error it will show.  
  * Read this file again and see if you didn't miss anything. Most likely the connection strings are not configured properly or there is something missing in the databases.

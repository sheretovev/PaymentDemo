cd ..\Demo.WebApi\bin\Debug\PublishOutput\
echo replace appsettings
C:\Windows\system32\inetsrv\appcmd stop apppool PaymentDemo
xcopy ".\*" "c:\inetpub\PaymentDemo\" /s /e /y /c
C:\Windows\system32\inetsrv\appcmd start apppool PaymentDemo
cd ..\..\..\..\..\..\..\Deploy
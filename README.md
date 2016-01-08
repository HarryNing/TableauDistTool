# TableauDistTool
Email Distribution Tool for Tableau Server

The Dist tool need to run on trusted host, 
run the following script on Tableau Server to configure Trusted Authentication.

--Configure Trusted Authentication
tabadmin stop 
tabadmin set wgserver.trusted_hosts "127.0.0.1, 192.168.0.128"
tabadmin config
tabadmin start

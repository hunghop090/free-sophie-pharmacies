# INSTALL SSL
https://docs.microsoft.com/en-us/dotnet/core/additional-tools/self-signed-certificates-guide

# Create a self-signed certificate
cd /Users/thuctran/Desktop/My_Projects/7.Sophie/sophie-backend/Sophie/Sophie/App.Security/Certificate
dotnet dev-certs https --clean
dotnet dev-certs https -ep ./https/aspnetapp.pfx -p galaxys6
dotnet dev-certs https --trust

OR:
security add-trusted-cert -d -r trustRoot -k /Library/Keychains/System.keychain ./https/aspnetapp.crt

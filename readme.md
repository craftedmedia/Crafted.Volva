# Crafted.Volva: Basic authentication module for IIS7 #

## Why a basic authentication module for IIS7? ##

In IIS6, ASP.Net is used more as a plugin to IIS, where requests are passed to the ASP.Net process after going through the IIS process. 

In II7, Microsoft introduced the concept of "integrated pipeline" and "Classic". Whereas Classic acts in the same way as IIS6, using "integrated pipeline" means that requests are handled by ASP.Net directly. 

In .Net, different authentication methods use the standard way of setting an authenticated user to the Page using the Principal. This then affects authentication, permissions rules set in web.config. 

In IIS6, it is possible to use IIS authentication (basic) to secure a site behind a password, which is useful for test sites. In IIS7 "integrated pipeline", ASP.Net handles all the authentication methods. 

This means that you can only set one method of authentication as there can be only one Principal set to the Page. So setting up Basic authentication will cause ASP.Net to consider the user as signed in as with Forms Authentication, which means that secured areas of the website no longer get redirected to a login page using Forms Authentication. 

## Usage ##

To use the module, simply include the DLLs in the project and modify your web.config in three places: 

In configSections, add a new config section for Crafted.Volva: 

```  
	<configSections>  
		<section name="Crafted.Volva" type="Crafted.Volva.VolvaConfig" />  
	</configSections>   
```  
 
In system.webServer (only as the issue only affects IIS7 in "integrated pipeline":

```  
	<system.webServer>  
		<modules runAllManagedModulesForAllRequests="true">  
			<remove name="Crafted.Volva"/>  
			<add name="Crafted.Volva" type="Crafted.Volva.BasicAuthentication" />  
		</modules>  
	</system.webServer>  
```  

And then the configuration settings:

```  
	<Crafted.Volva BasicAuthenticationIsEnabled="true">  
		<locations>  
			<path match="*" />  
		</locations>  
		<users>  
			<user username="sampleUser" password="samplePassword" />  
		</users>  
	</Crafted.Volva>  
```  

## Settings ##

`<locations>` refer to which parts of the site should be secured by Crafted.Volva. The paths include either "*" (for all requests) or a regex expression passed against the Request.RawUrl. 

`<users>` refer to additional users you want to set. Note that users that have a server login can log in with these credentials. 

## Removal ##

Removal is easy. You can do this in multiple ways:  
1. Remove all web.config settings and the DLL  
2. Remove all web.config settings only and keep the DLL  
3. Remove the configuration section in web.config   
4. Remove the HTTP Module line in web.config  
5. Set BasicAuthenticationIsEnabled="false"  

The last two ways of removing are advised as this allows you to use the module whenever you need by simply re-adding the HTTP module or changing the config setting.  
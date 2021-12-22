# Technology stack

* dotnet core
    * because of the most know how, otherwise we could use spring boot
    * deployable everywhere
* angular
    * again because of know how, and some kind of dynamic web frontend is kind of a must

# Deployment scenario

Somewhere in the cloud I guess:
https://docs.aws.amazon.com/elasticbeanstalk/latest/dg/dotnet-core-tutorial.html

Would be interesting to use ARM - infrastructure as code
https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/template-tutorial-create-first-template?tabs=azure-cli

# Communication channels

According to Odo following are used by teenagers:

* Email
* WhatsApp
* Instagram (API in beta)
* Tiktok (DM only for > 16 yo)
* Snapchat (only reverse engineering API)
* (Facebook) (only via FB App)

## Email templating

* Allowing upload of a template (html+content) as a zip
* Online editing with TinyMCE - https://www.tiny.cloud/blog/email-marketing-software/
* Allow adding tokens via TinyMCE

### Further features

* Send preview mail with filled out tokens

## WhatsApp

API only available to bigger companies, basically we have to go via third party like Twilio or any other of those: https://www.facebook.com/business/partner-directory/search?platforms=whatsapp&solution_type=messaging

## Facebook Messenger

API available but the messaging is linked to a FB app and therefor not so much useable: https://developers.facebook.com/docs/messenger-platform/getting-started

## Instagram

Soon possible over the same APIs as FB Messenger: https://www.facebook.com/business/news/messenger-api-to-support-instagram

## Tiktok

Messaging is prohibited in our target group. And it's only possible to send msg to users following you anyway. https://www.bbc.com/news/technology-52310529
And no proper API available.

## Snapchat

There seems to be no official API supporting sending messages to Snap: https://kit.snapchat.com/

# Infrastructure requirements

* Mail server that allows sending from any domain
* Some good fake domains with proper certificates

# Architecture

## Layers

* SPA Frontend
* dotnet core backend
* SQL database

## User Roles

* Campaign manager
* User
* Admin

## General application structure

A few crud masks for the basic entities:

* campaign (start, end, )
    * campaign association
* phishing template
* user actions
* users
    * social metadata eg. pet, birthday etc.

# User stories

## Create campaign

As a campaign manager I want to start a campaign.

## Edit campaign metadata

As a campaign manager I want to edit the metadata (start, end, name, etc., number of messages per user) of a campaign.
As a campaign manager I want to select existing phishing templates to be used in the campaign and select some timespan when this templated is used.
As a ... I want to select the channels to use for the ...

## Invite users

As a campaign manager I want to invite users to a existing campaign via link (later email etc).

## User creation

As a user I want to create a user account (via email/password or external login).

## User social data

As a user I want to edit my social logins and metadata.

## User enrollment

As a user I want to participate in a campaign I was invited to.

## Phishing messages

As a campaign manager I want phishing messages to be sent to users at random points in time.

## Phishing templates

As a campaign manager I want to upload phishing templates to be assiciated with a campaign and define the target channel.
As a admin I want to upload phishing templates for general availability and define the target channel and a domain for the link.
As the system I want to analyze the uploaded templates and extract the required sozial metadata.

## Logging

As the system I want to log the activity (sent time, template name, channel, user).
As the system I want to log the user interaction with template embedded links (timestamp).

## Reporting

As a campaign manager I want to check the latest activity of all the campaign users.

## Cleanup

As a campaign manager I want to be able to clean all the (personal) data associated with a campain, and anonymize the non personal data at the end manualy.

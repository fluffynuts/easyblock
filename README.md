#EasyBlock

##What is it?

EasyBlock is a Win32 service which aspires to block most advertising through hostfile
redirections for known ad servers. It's inspired heavily by AdAway for Android
(http://adaway.org) because of the difference in user experience that AdAway made on
my phone, though I haven't actually looked at how AdAway achieves it's goal.

##Why?

I don't have a problem with advertising on the internet. I do have a problem with how
advertisers abuse the space and users on the internet. Most of these problems aren't
from advertising self-hosted by sites -- it's from the ad servers which don't vet
the content and usage. Specifically, these kinds of behaviours ruin the internet for all:

- Adverts which distract to the point of making site content difficult to digest
    - animated adverts
    - auto-playing videos
    - flash (can't it die yet?)
- Adverts which cover the overwhelming majority of the page, making content difficult to read
- Adverts which just plain lie
    - multiple "download" buttons on software host sites
    - scareware links (the ones warning you that your IP is known, for instance)
- Adverts serving as malware vectors
    - the most recent was a spread of ransomware through ad-hosts
- The general slowing of website loading due to advertising overload
    - especially ads which require Javascript. Javascript must be downloaded in serial when embedded in a web page so multiple external Javascript links really cause load times to suffer
    - of course, these also have an impact on battery life for mobile devices; whilst EasyBlock is (at the moment) aimed at Windows PC devices, that would also include laptops and tablets.

I believe that it's time for website operators to take responsibility for the content
on their sites, to vet them for the above problems. They can only do so by hosting the
content themselves, and this, for me, is why a host-based blocking approach seems like
a reasonable compromise: this approach only blocks the big players who are more interested
in earning ad revenues than in the harm they allow to befall unwary internet users.


##How?

EasyBlock was developed TDD-style towards the primary functional requirements that AdAway provides:

- periodically download hosts files
- merge into C:\windows\system32\drivers\etc\hosts, in section bounded by comments
- get hosts file from AdAway providers to start:
    - https://adaway.org/hosts.txt 
    - http://winhelp2002.mvps.org/hosts.txt
    - http://hosts-file.net/ad_servers.txt
    - https://pgl.yoyo.org/adservers/serverlist.php?hostformat=hosts&showintro=0&mimetype=plaintext 
- provide configuration mechanism for
    - blacklists
    - whitelists
    - sources
- the service patches the hosts file on start (and periodically updates)
- the service unpatches the hosts file on stop, leaving the system in the original state

There is a thought to provide a configuration GUI later on, should interest be high enough.
Configuration is via INI file, so most people should be able to tweak it easily enough and
the default configuration should be good enough for most people.

##Licensing

EasyBlock is open-source and free to use, distribute and modify. It's available under the BSD
license and I'm open for well-tested, useful pull requests. EasyBlock has 100% test coverage
at time of writing!

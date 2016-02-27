EasyBlock
---------

Win32 service (using PB.ServiceShell)

- inspired by AdAway for Android
- periodically download hosts files
- merge into C:\windows\system32\drivers\etc\hosts, in section bounded by comments
- get hosts file from AdAway providers to start:
    - https://adaway.org/hosts.txt 
    - http://winhelp2002.mvps.org/hosts.txt
    - http://hosts-file.net/ad_servers.txt
    - https://pgl.yoyo.org/adservers/serverlist.php?hostformat=hosts&showintro=0&mimetype=plaintext 
- provide textfile mechanism for
    - blacklists
    - whitelists
    - sources configuration
- later on, provide GUI for configuration of blacklists, whitelists, sources
- service must patch hosts file on start
- service must unpatch hosts file on stop
    - means that stopping the service returns the host machine back to normal
    - user could stop & disable if they wanted to
    - uninstall is simplified


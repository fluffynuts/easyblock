# EasyBlock 

## Once-off run

`EasyBlock.Win32Service.exe -r`

_Remember: in once-off mode, EasyBlock will not revert your hosts file
after completion. It's up to you to do so. EasyBlock will append entries
to the hosts file with a comment above the entries so it's easy to find
what needs to be removed: normal service operation will delete all hosts
entries after the magic comment._

## Installation instructions

1. Unpack this archive somewhere you'd like to keep the application
2. run `EasyBlock.Win32Service.exe --install`
3. run `net start easyblock`

EasyBlock will:
- create an EasyBlock.ini file if it is missing
  - you can easily reset configuration by simply deleting
    your EasyBlock.ini file
- periodically download host entries from remote sources to block via
  localhost redirect
  - EasyBlock will configure itself with some good default sources
    but you can add or remove sources in the EasyBlock.ini file
  - EasyBlock will, by default, poll every 1440 seconds (24 hours)
    but you can change this in the EasyBlock.ini file
- add these hosts to your system-wide hosts file
  - if you're running an operating system other than Windows,
    you may modify the EasyBlock.ini file to set the host file location
  - EasyBlock should run under Mono in single-run mode (ie with the
    `-r` commandline argument. You will need to cron this and will
    be responsible for hostfile cleanup if necessary.
- remove them when the service is stopped

So you should have passive blocking whilst the service is running,
but not when it is stopped.

## Uninstall
run `EasyBlock.Win32Service.exe --uninstall`

## Operation notes:

EasyBlock will modify your system hosts file to override known ad
server dns entries to be rebound to localhost (127.0.0.1). This should
make your life better: expect fewer ads and quicker-loading pages.
However, there are bound to be scenarios where web applications
will not behave as expected due to this behavior. As an example,
you may occasionally find that a link on Twitter may fail to load
(the first time) as it will attempt to pass through a tracking
service. My experience is that going back to Twitter and clicking
the link again will work as the Twitter website only wants to track
unique users of tracked links.

If you find that there is a site which you'd like to allow but
which is blocked by one of the adserver sources, you may add it
to the `[whitelist]` section. Similarly, if there's a host you'd like
to specifically block which is not already handled by one of your
upstream lists, you may add it to the `[blacklist]` section. Black-
and whitelist hosts are added one per line.

If you suspect that EasyBlock is interfering with operations that
you'd like to allow, simply stop the service and try again. If
things work as you expect, you may need to add the offending host
to your `[whitelist]` configuration. For example, if you were
visiting a link like:

`http://some.ad.server/some/path/to/an/advert`

Then you would add

`some.add.server`

to the `[whitelist]` section of your EasyBlock.ini file, found
in the EasyBlock installation folder.


[Find EasyBlock on GitHub](https://github.com/fluffynuts/easyblock)

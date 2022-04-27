[![Build status](https://github.com/icalvo/sched/actions/workflows/pull-request.yml/badge.svg)](https://github.com/icalvo/sched/actions/workflows/pull-request.yml)
![Nuget](https://img.shields.io/nuget/v/sched)
![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/sched?label=nuget%20pre)

# 📆 sched
**sched** is a general purpose task scheduler.

It is an enhanced version of cron scheduler. A sched configuration file contains _several_ crontab-like groups. Each group applies to an specific interval that can be:
* A single day
* An interval of days, where both endpoints can be open.

The groups are then layered in the order they are read from the configuration file, thus the latest one takes precedence when considering which crontab group should be operating at each moment.

## <a id="Installation"></a>Installation

```
> dotnet tool install --global sched
```

## Uninstallation

```
> dotnet tool install --global sched
```
After uninstalling, no configuration file will be deleted.

## Configuration and usage
Please head to the [wiki page](https://github.com/icalvo/sched/wiki) to find documentation on configuration and usage.

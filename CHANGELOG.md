# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.3.0] - 2022-05-22
### Added
- [Notifications](https://github.com/icalvo/sched/wiki/Home/_edit#notifications)
- Scheduled processes now show stderr in addition to stdout
- [Process timeout](https://github.com/icalvo/sched/wiki#process-timeout)

## [1.2.1] - 2022-05-10
### Fixed
- Wait for the next event was wrong when the machine was suspended.

## [1.2.0] - 2022-05-07
### Added
- Allowed date interval lines without a starting dash

## [1.1.0] - 2022-05-06
### Added
- [Randomization of times](https://github.com/icalvo/sched/wiki#time-randomization) configurable in the config file

## [1.0.0] - 2022-04-28
### Added
- Interval groups of cron schedules
- Intervals of single day, between dates, from date, to date, or all dates
- Interval overlapping
- Inverted cron format to enhance readability
- Command aliases

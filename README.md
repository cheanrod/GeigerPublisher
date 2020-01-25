# GeigerPublisher

![GitHub release (latest SemVer including pre-releases)](https://img.shields.io/github/v/release/cheanrod/GeigerPublisher?include_prereleases)
[![docker pulls][badge_docker]][link_docker]
[![mqtt-smarthome](https://img.shields.io/badge/mqtt-smarthome-blue.svg)](https://github.com/mqtt-smarthome/mqtt-smarthome)
![Build Status](https://github.com/cheanrod/GeigerPublisher/workflows/Build%20Status/badge.svg)
![GitHub](https://img.shields.io/github/license/cheanrod/GeigerPublisher)

GeigerPublisher publishes serial readings from [Mightyohm Geiger Counter](https://mightyohm.com/blog/products/geiger-counter/) to MQTT.

Usage:

```bash
GeigerPublisher <serial port> <broker hostname>
```

_broker hostname_ can be either IP address or hostname.

[badge_docker]: https://img.shields.io/docker/pulls/cheanrod/geigerpublisher
[link_docker]: https://hub.docker.com/r/cheanrod/geigerpublisher

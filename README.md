# azmsgctl

`azmsgctl` is a command-line interface (CLI) tool designed to manage Azure Service Bus messages. It provides functionalities to delete and resend dead letter messages and cancel scheduled messages.

## Installation

Download the utility [here](https://github.com/gui-abrantes/azure-queue-manager/releases) according to your operating system

## Usage

The basic usage of the `azmsgctl` tool is as follows:

```
azmsgctl [options]
azmsgctl [command] [...]
```

### Options

- `-h|--help`: Shows help text.
- `--version`: Shows version information.

#### Setting the `--connectionstring` option as an environment variable

As a way to hide the connection string (**remembering that it has to be from NAMESPACE**) we can set it as a console environment variable. The name of variable `SB_CONNSTR`.

#### Examples
Powersheel (Windows):
```
$Env:SB_CONNSTR = "Endpoint=sb://<namespace>.servicebus.windows.net/;SharedAccessKeyName=<keyname>;SharedAccessKey=<key>"
```

Linux:
```
export SB_CONNSTR="Endpoint=sb://<namespace>.servicebus.windows.net/;SharedAccessKeyName=<keyname>;SharedAccessKey=<key>"
```

### Commands

#### deadletter delete

Deletes dead letter messages from the specified entity.

##### Usage

```
azmsgctl deadletter delete --entity <value> --connectionstring <value> [options]
```

##### Description

Delete dead letter messages.

##### Options

- `-e|--entity`: Queue/topic that should be processed.
- `--connectionstring`: Azure Service ConnectionString - NAMESPACE Environment variable: `SB_CONNSTR`.
- `-s|--subscription`: Subscription of the topic. Default: "".
- `--prefetchcount`: Local Prefetch Buffer. Default: "1000".
- `--messagecount`: Receive Message Count. Default: "100".
- `--timeout`: Receiver timeout in seconds. Default: "10".
- `-h|--help`: Shows help text.

#### deadletter resend

Resends dead letter messages from the specified entity.

##### Usage

```
azmsgctl deadletter resend --entity <value> --connectionstring <value> [options]
```

##### Description

Resend dead letter messages.

##### Options

- `-e|--entity`: Queue/topic that should be processed.
- `--connectionstring`: Azure Service ConnectionString - NAMESPACE Environment variable: `SB_CONNSTR`.
- `-s|--subscription`: Subscription of the topic. Default: "".
- `--prefetchcount`: Local Prefetch Buffer. Default: "1000".
- `--messagecount`: Receive Message Count. Default: "100".
- `--timeout`: Receiver timeout in seconds. Default: "10".
- `-h|--help`: Shows help text.

#### scheduled cancel

Cancels scheduled messages from the specified entity.

##### Usage

```
azmsgctl scheduled cancel --entity <value> --connectionstring <value> [options]
```

##### Description

Cancel scheduled messages.

##### Options

- `-e|--entity`: Queue/topic that should be processed.
- `--connectionstring`: Azure Service ConnectionString - NAMESPACE Environment variable: `SB_CONNSTR`.
- `-s|--subscription`: Subscription of the topic. Default: "".
- `--prefetchcount`: Local Prefetch Buffer. Default: "1000".
- `--messagecount`: Receive Message Count. Default: "100".
- `--timeout`: Receiver timeout in seconds. Default: "10".
- `-h|--help`: Shows help text.

## Examples

### Deleting Dead Letter Messages

To delete dead letter messages from a specific entity:

```
azmsgctl deadletter delete --entity myQueue --connectionstring "Endpoint=sb://<namespace>.servicebus.windows.net/;SharedAccessKeyName=<keyname>;SharedAccessKey=<key>"
```
Connection string as an environment variable:
```
azmsgctl deadletter delete --entity myQueue"
```

### Resending Dead Letter Messages

To resend dead letter messages from a specific entity:

```
azmsgctl deadletter resend --entity myQueue --connectionstring "Endpoint=sb://<namespace>.servicebus.windows.net/;SharedAccessKeyName=<keyname>;SharedAccessKey=<key>"
```

### Cancelling Scheduled Messages

To cancel scheduled messages from a specific entity:

```
azmsgctl scheduled cancel --entity myQueue --connectionstring "Endpoint=sb://<namespace>.servicebus.windows.net/;SharedAccessKeyName=<keyname>;SharedAccessKey=<key>"
```

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

Feel free to modify this draft as needed!
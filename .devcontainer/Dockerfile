FROM mcr.microsoft.com/devcontainers/base:bookworm

# Install C/C++
RUN apt update && \
    apt -y install build-essential \
                   gcc \
                   g++ \
                   gdb

# Install Lua
RUN apt install -y lua5.4

# Install Python 3.11
RUN apt install -y python3.13 \
                   python3.13-dev \
                   python3-distutils \
                   python3-pip

# Install Node.js
RUN apt install -y nodejs npm

# Install Rust
# RUN apt install -y rustc cargo rust-src rustfmt rust-gdb rust-lldb rust-doc rust-all
RUN runuser -l vscode -c 'curl --proto "=https" --tlsv1.2 -sSf https://sh.rustup.rs | sh -s -- -y' && \
    runuser -l vscode -c 'source $HOME/.cargo/env && rustup component add rust-src rustfmt rust-docs rust-analysis'

# Install .NET SDK (TODO: set path only for vscode user)
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV PATH="${PATH}:/home/vscode/.dotnet:/home/vscode/.dotnet/tools"
RUN wget https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && rm packages-microsoft-prod.deb \
    && apt update \
    && apt install -y dotnet-sdk-8.0 dotnet-sdk-9.0 \
    && runuser -l vscode -c 'dotnet tool install -g --version=1.6.0 dotnet-script'

# Temp manual install .NET 10 RC to be able to use file based apps (until it's available via apt)
# source: https://github.com/dotnet/core/blob/main/release-notes/10.0/preview/preview6/sdk.md#file-based-apps
COPY temp-dotnet-sdk-10-rc-linux-x64.tar.gz /tmp/temp-dotnet-sdk-10-rc-linux-x64.tar.gz
RUN tar xzf /tmp/temp-dotnet-sdk-10-rc-linux-x64.tar.gz -C /usr/share/dotnet \
    && rm /tmp/temp-dotnet-sdk-10-rc-linux-x64.tar.gz

# Install Go
RUN apt install -y golang gopls

# Clean up
RUN apt autoremove -y && \
    apt clean -y && \
    rm -rf /var/lib/apt/lists/*

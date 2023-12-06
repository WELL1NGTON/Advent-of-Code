FROM mcr.microsoft.com/devcontainers/base:bookworm

# Install C/C++
RUN apt update \
    && apt -y install build-essential gcc g++ gdb

# Install Lua
RUN apt install -y lua5.4

# Install Python 3.11
RUN apt install -y python3.11 python3.11-dev python3-distutils python3-pip

# Install Node.js
RUN apt install -y nodejs npm

# Install Rust
# RUN apt install -y rustc cargo rust-src rustfmt rust-gdb rust-lldb rust-doc rust-all
RUN runuser -l vscode -c 'curl --proto "=https" --tlsv1.2 -sSf https://sh.rustup.rs | sh -s -- -y' \
    && runuser -l vscode -c 'source $HOME/.cargo/env && rustup component add rust-src rustfmt rust-docs rust-analysis'

# Install .NET SDK (TODO: set path only for vscode user)
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV PATH="${PATH}:/home/vscode/.dotnet:/home/vscode/.dotnet/tools"
RUN wget https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && rm packages-microsoft-prod.deb \
    && apt update \
    && apt install -y dotnet-sdk-8.0 aspnetcore-runtime-8.0 \
    && runuser -l vscode -c 'dotnet tool install -g --version=1.5.0 dotnet-script'

# Install Go
RUN apt install -y golang \
    && runuser -l vscode -c 'go install -v golang.org/x/tools/cmd/goimports@latest' \
    && runuser -l vscode -c 'go install -v golang.org/x/tools/gopls@latest' \
    && runuser -l vscode -c 'go install -v github.com/go-delve/delve/cmd/dlv@latest' \
    && runuser -l vscode -c 'go install -v honnef.co/go/tools/cmd/staticcheck@latest'

# Clean up
RUN apt autoremove -y \
    && apt clean -y \
    && rm -rf /var/lib/apt/lists/*

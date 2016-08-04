var Dialog = React.createClass({
    getInitialState: function() {
        return {
            visible: false
        }
    },
    render: function(){
        return(
            <div className="dialog-all">
                <div className="dialog-mask"></div>
                <div className="dialog-wrap">
                    <div className="dialog" style={this.props.style}>
                        <div className="dialog-content">
                            <button className="dialog-close" onClick={this.props.onClose}>
                                <span className="dialog-close-x"></span>
                            </button>
                            <div className="dialog-header">
                                 {this.props.title}
                            </div>
                            <div className="dialog-body">
                                {this.props.children}
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
});

var PublishMessageButton = React.createClass({
    displayName: 'PublishMessageButton',
    getInitialState() {
        return {
          visible: false,
          width: 600
        };
    },
    handlePublishLinkClick: function(e) {
        this.setState({
            visible: true,
        });
    },
    onClose: function(e) {
        this.setState({
            visible: false,
        });
    },
    render: function() {
        let dialog;
        if (this.state.visible) {
            const style = {
               width: this.state.width,
            };

            dialog = (
                <Dialog visible={this.state.visible}
                        onClose={this.onClose}
                        style={style}
                        title={<div>发布消息</div>}
                >
                    <input />
                    <p>basic modal</p>
                    <div style={{ height: 500 }}></div>
                </Dialog>
            );
        }
        return (
            <div>
                <a className="publishMessageButton" onClick={this.handlePublishLinkClick} href="javascript:void(0)">发布消息</a>
                {dialog}
            </div>
            
        );
    }
});

var MessageList = React.createClass({
    displayName: 'MessageList',
    render: function() {
        return (
            <div className="messageList">
            Hello, world! I am a MessageList.
            </div>
        );
    }
});

var MessageBox = React.createClass({
    displayName: 'MessageBox',
    render: function () {
        return (
            <div className="messageBox">
                <PublishMessageButton />
                <MessageList />
            </div>
        );
    }
});
ReactDOM.render(
    <MessageBox />,
    document.getElementById('MessageBox')
);
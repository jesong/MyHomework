
//import 'rc-dialog/assets/index.css';
//import React from 'react';
//import ReactDOM from 'react-dom';
//import Dialog from 'rc-dialog';

var PublishMessageButton = React.createClass({
    displayName: 'PublishMessageButton',
    handlePublisLinkClick: function(e) {
        alert("hello");
    },
    onClose: function(e) {

    },
    render: function() {
        return (
            <a className="publishMessageButton" onClick={this.handlePublisLinkClick} href="javascript:void(0)">发布消息</a>

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
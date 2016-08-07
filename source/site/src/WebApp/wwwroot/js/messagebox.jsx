var Dialog = React.createClass({
    displayName: 'dialog',
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

var File = React.createClass({
    displayName: 'File',
    handleRemoveFile: function(e)
    {
        this.props.onRemoveFile({ link: this.props.fileLink });
    },
    render: function () {
        return (
            <div className="file-wrapper">
                <a className="file-name" href={this.props.fileLink}>{this.props.fileName}</a>
                <a className="remove-file-button" href="javascript:void(0)" onClick={this.handleRemoveFile}>
                    <span className="remove-file-icon"></span>
                </a>
            </div>
        );
    }
});

var FileUploader = React.createClass({
    displayName: 'FileUploader',
    getInitialState: function () {
        return {
            data: [],
            uploading: false,
        }
    },
    handleUploadFile: function(e) {
        if (this.refs.file.files.length == 1) {
            var currentfiles = this.state.data;

            var file = this.refs.file.files[0];

            var existingFiles = $.grep(currentfiles, function (value) {
                return value.name == file.name;
            });

            if (existingFiles.length > 0) {
                alert("文件名已经存在，无法上传同名文件！");
                this.refs.file.value = '';
                return;
            }

            var data = new FormData();
            data.append(file.name, file);
            this.refs.file.value = '';
            this.setState({ uploading: true });

            $.ajax({
                type: "POST",
                url: "/homeworkpublish/uploadfile",
                contentType: false,
                processData: false,
                data: data,
                timeout: 600000,
                cache: false,
                success: function (message) {
                    currentfiles.push({
                        name: message[0].fileName,
                        link: message[0].url
                    });

                    this.setState({ data: currentfiles, uploading: false });
                }.bind(this),
                error: function () {
                    this.setState({ uploading: false });
                    alert("上传文件错误！\r\n1. 文件可能太大了\r\n2. 服务器超时\r\n3. 其他原因");
                }.bind(this)
            });
        }
    },
    handleRemoveFile: function(file) {
        var currentfiles = this.state.data;
        currentfiles = $.grep(currentfiles, function (value) {
            return value.link != file.link;
        });
        this.setState({ data: currentfiles });
    },
    render: function () {
        var removeHandler = this.handleRemoveFile;
        var files = this.state.data.map(function(file) {
            return (
                <File fileLink={file.link} fileName={file.name} onRemoveFile={removeHandler} />
            );
        });
        return (
            <div className="file-uploader-wrap">
                {files}
                <div className="file-uploader">
                    <label htmlFor="file-upload-control" className="file-uploader-custom-file-upload">
                        {this.state.uploading ? "努力上传文件中..." : "点这里上传文件"}
                        
                    </label>
                    <input id="file-upload-control" ref="file" type="file" name="uploadFile" className="file-uploader-choose-file" onChange={this.handleUploadFile}  disabled={this.state.uploading}/>
                </div>
            </div>
        );
    }
});

var MessagePublisher = React.createClass({
    displayName: 'MessagePublisher',
    render: function () {
        return (
            <form className="message-publisher-form">
                <input className="message-publisher-title" placeholder="标题" />
                <textarea className="message-publisher-content" placeholder="内容" rows="10"/>
                <FileUploader />
                <input className="message-publisher-submit" type="submit" value="提交" />
                <div className="clear"></div>
            </form>
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
                    <MessagePublisher />
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
            <div className="message-box">
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
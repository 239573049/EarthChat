import React, { Component } from 'react';

interface VideoPlayerProps {
  url: string;
}

class VideoPlayer extends Component<VideoPlayerProps> {

  constructor(props: VideoPlayerProps) {
    super(props);
  }

  componentDidMount() {
  }

  componentWillUnmount() {
  }

  render() {
    const { url } = this.props;

    return (<video
        style={{
          borderRadius: 8,
          width: '400px',
          cursor: 'pointer',
        }}
        src={url}
        controls
        autoPlay={false}
      ></video>
    );
  }
}

export default VideoPlayer;

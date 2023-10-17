import React, { Component } from 'react';

interface VideoPlayerProps {
  url: string;
  float: any,
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
    const { url, float } = this.props;

    return (<video
      style={{
        borderRadius: 8,
        width: '400px',
        cursor: 'pointer',
        height: '300px',
        float: float
      }}
      src={url}
      controls
      autoPlay={false}
    ></video>
    );
  }
}

export default VideoPlayer;

import React, { Component } from 'react';

export class Home extends Component {
    static displayName = Home.name;

    constructor(props) {
        super(props);
        this.state = {
            cardShortSideInInches: 2.5,
            cardLongSideInInches: 3.5,
            bleedSizeInInches: 0.125,
            borderPaddingInInches: .0625,
            saveConfiguration: 'Page',
            generating: false,
            error: null,
            downloadLink: null
        };
    }

    handleCardShortSideInInchesChange = (event) => {
        this.setState({ cardShortSideInInches: +event.target.value });
    }
    handleCardLongSideInInchesChange = (event) => {
        this.setState({ cardLongSideInInches: +event.target.value });
    }
    handleBleedSizeInInchesChange = (event) => {
        this.setState({ bleedSizeInInches: +event.target.value });
    }
    handleBorderPaddingInInchesChange = (event) => {
        this.setState({ borderPaddingInInches: +event.target.value });
    }
    handleSaveConfigurationChange = (event) => {
        this.setState({ saveConfiguration: event.target.value });
    }

    handleGenerateClick = async () => {
        var {
            cardShortSideInInches,
            cardLongSideInInches,
            bleedSizeInInches,
            borderPaddingInInches,
            saveConfiguration
        } = this.state;
        this.setState({ generating: true, error: null, downloadLink: null });
        const body = JSON.stringify({
            cardShortSideInInches,
            cardLongSideInInches,
            bleedSizeInInches,
            borderPaddingInInches,
            saveConfiguration
        });
        try {
            var response = await fetch('Gtr/GenerateImages', {
                method: 'POST',
                body,
                headers: { 'Content-Type': 'application/json', },
            });
            if (response.ok) {
                var downloadLink = await response.text();
                this.setState({ generating: false, downloadLink });
            }
            else
                this.setState({ generating: false, error: response.statusText });

        } catch (error) {
            this.setState({ generating: false, error });
        }
    }

    render() {
        var {
            cardShortSideInInches,
            cardLongSideInInches,
            bleedSizeInInches,
            borderPaddingInInches,
            saveConfiguration,
            generating,
            error,
            downloadLink
        } = this.state;
        return (
            <>
                <div>
                    <input type='number' value={cardShortSideInInches} onChange={this.handleCardShortSideInInchesChange} disabled={generating}/>
                    <input type='number' value={cardLongSideInInches} onChange={this.handleCardLongSideInInchesChange} disabled={generating}/>
                </div>
                <div>
                    <input type='number' value={bleedSizeInInches} onChange={this.handleBleedSizeInInchesChange} disabled={generating}/>
                    <input type='number' value={borderPaddingInInches} onChange={this.handleBorderPaddingInInchesChange} disabled={generating}/>
                </div>
                <div>
                    <select value={saveConfiguration} onChange={this.handleSaveConfigurationChange} disabled={generating}>
                        <option>Page</option>
                        <option>SingleImage</option>
                    </select>
                </div>
                <div>
                    {!generating && <button type='button' className='btn btn-primary' onClick={this.handleGenerateClick}>Generate Labels</button>}
                    {generating && <button type='button' className='btn btn-primary disabled'>Generating...</button>}
                    {downloadLink != null && <h3>Generated File: <a href={downloadLink}>Link</a> (Link valid for 1 day)</h3>}
                    {error != null && <div>Error generating file or uploading to S3: {error}</div>}
                </div>
            </>
        );
    }
}

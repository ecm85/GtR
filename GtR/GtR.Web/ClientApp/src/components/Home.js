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
            <form>
                <div className='card'>
                    <div className='card-body'>
                        <div className='form-row'>
                            <div className='col'>
                                <div className='form-group'>
                                    <label htmlFor='cardShortSideInInches'>
                                        Card short side, in inches
                                    </label>
                                    <input id='cardShortSideInInches' className='form-control' type='number' value={cardShortSideInInches} onChange={this.handleCardShortSideInInchesChange} disabled={generating} />
                                </div>
                            </div>
                            <div className='col'>
                                <div className='form-group'>
                                    <label htmlFor='cardLongSideInInches'>
                                        Card long side, in inches
                                    </label>
                                    <input id='cardLongSideInInches' className='form-control' type='number' value={cardLongSideInInches} onChange={this.handleCardLongSideInInchesChange} disabled={generating} />
                                </div>
                            </div>
                        </div>
                        <div className='form-row'>
                            <div className='col'>
                                <div className='form-group'>
                                    <label htmlFor='bleedSizeInInches'>
                                        Bleed size, in inches (default is 1/8th of an inch)
                                    </label>
                                    <input id='bleedSizeInInches' className='form-control' type='number' value={bleedSizeInInches} onChange={this.handleBleedSizeInInchesChange} disabled={generating} />
                                </div>
                            </div>
                            <div className='col'>
                                <div className='form-group'>
                                    <label htmlFor='borderPaddingInInches'>
                                        Border padding, in inches (default is 1/16th of an inch)
                                    </label>
                                    <input id='borderPaddingInInches' className='form-control' type='number' value={borderPaddingInInches} onChange={this.handleBorderPaddingInInchesChange} disabled={generating} />
                                </div>
                            </div>
                        </div>
                        <div className='form-row'>
                            <div className='col'>
                                <div className='form-group'>
                                    <label htmlFor='saveConfiguration'>
                                        Bleed size, in inches (default is 1/8th of an inch)
                                    </label>
                                    <select id='saveConfiguration' className='form-control' value={saveConfiguration} onChange={this.handleSaveConfigurationChange} disabled={generating}>
                                        <option>Page</option>
                                        <option>SingleImage</option>
                                    </select>
                                </div>
                            </div>
                        </div>
                        <div>
                            {!generating && <button type='button' className='btn btn-primary form-control' onClick={this.handleGenerateClick}>Generate Images</button>}
                            {generating && <button type='button' className='btn btn-primary disabled form-control'>Generating...</button>}
                        </div>
                    </div>
                    {downloadLink != null && <div className='card-footer'><p className='card-text'><a className='card-link' href={downloadLink}>Download generated file</a> (Link valid for 1 day)</p></div>}
                    {error != null && <div className='card-footer'><p className='card-text'>Error generating file or uploading to S3.</p></div>}
                </div>
            </form>
        );
    }
}

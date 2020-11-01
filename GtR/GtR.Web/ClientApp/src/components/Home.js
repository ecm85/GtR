import React, { Component } from 'react';

export class Home extends Component {
    static displayName = Home.name;

    render () {
        return (
            <form method='post' action='Gtr/GenerateImages'>
                <div className='form-group'>
                    <input type='number' name='cardShortSideInInches' id='cardShortSideInInches' defaultValue='2.5' />
                    <input type='number' name='cardLongSideInInches' id='cardLongSideInInches' defaultValue='3.5' />
                </div>
                <div className='form-group'>
                    <input type='number' name='bleedSizeInInches' id='bleedSizeInInches' defaultValue='0.125'/>
                    <input type='number' name='borderPaddingInInches' id='borderPaddingInInches' defaultValue='0.0625' />
                </div>
                <div className='form-group'>
                    <select name='saveConfiguration' id='saveConfiguration' defaultValue='Page'>
                        <option>Page</option>
                        <option>SingleImage</option>
                    </select>
                </div>
                <div className='form-group'>
                    <input type='submit' className='btn btn-primary' value='Generate Images' />
                </div>
            </form>
        );
    }
}
